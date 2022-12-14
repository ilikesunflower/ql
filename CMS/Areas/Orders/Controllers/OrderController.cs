using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using ClosedXML.Report;
using CMS_Access.Repositories.Customers;
using CMS_Access.Repositories.Orders;
using CMS_Access.Repositories.Products;
using CMS_EF.Models.Customers;
using CMS_EF.Models.Orders;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using CMS.Areas.Customer.Services;
using CMS.Areas.Orders.Const;
using CMS.Areas.Orders.Models;
using CMS.Areas.Orders.Servers;
using CMS.Areas.Orders.Services;
using CMS.Config.Consts;
using CMS.Controllers;
using CMS.Models;
using CMS.Models.ModelContainner;
using CMS.Services.Files;
using CMS_Access.Repositories.WareHouse;
using CMS_EF.Models.WareHouse;
using CMS_WareHouse.KiotViet;
using CMS_WareHouse.KiotViet.Consts;
using CMS.Areas.PointInput.Const;
using CMS.Areas.Reports.Models.SummaryReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Orders.Controllers;

[Area("Orders")]
[Obsolete]
public class OrderController : BaseController
{
    private readonly ILogger<OrderController> _iLogger;
    private readonly IOrderServer _iOrderServer;
    private readonly ICustomerCouponRepository _iCustomerCouponRepository;
    private readonly ICustomerRepository _iCustomerRepository;
    private readonly ICustomerAddressRepository _iCustomerAddressRepository;
    private readonly IProductRepository _iProductRepository;
    private readonly IOrdersRepository _iOrdersRepository;
    private readonly IFileService _fileService;
    private readonly IProductSimilarRepository _iProductSimilarRepository;
    private readonly IOrderService _iOrderService;
    private readonly IOrdersAddressRepository _iOrdersAddressRepository;
    private readonly IOrderProductRepository _iOrderProductRepository;
    private readonly ICustomerNotificationService _iCustomerNotificationService;
    private readonly IWhTransactionRepository _iWhTransactionRepository;
    private readonly IKiotVietService _iKiotVietService;
    private readonly IWebHostEnvironment _iHostingEnvironment;

    private double IsPoi = PointConst.Coefficient;

    public OrderController(ILogger<OrderController> iLogger, IOrderServer iOrderServer,
        ICustomerCouponRepository iCustomerCouponRepository,
        ICustomerRepository iCustomerRepository, ICustomerAddressRepository iCustomerAddressRepository,
        IProductRepository iProductRepository,
        IFileService fileService, IOrdersRepository iOrdersRepository,
        IProductSimilarRepository iProductSimilarRepository, IOrderService iOrderService,
        IOrdersAddressRepository iOrdersAddressRepository, IOrderProductRepository iOrderProductRepository,
        ICustomerNotificationService iCustomerNotificationService, IWhTransactionRepository iWhTransactionRepository,
        IKiotVietService iKiotVietService, IWebHostEnvironment iHostingEnvironment)
    {
        _iLogger = iLogger;
        _iOrderServer = iOrderServer;
        _iCustomerCouponRepository = iCustomerCouponRepository;
        _iCustomerRepository = iCustomerRepository;
        _iCustomerAddressRepository = iCustomerAddressRepository;
        _iProductRepository = iProductRepository;
        _fileService = fileService;
        _iOrdersRepository = iOrdersRepository;
        _iProductSimilarRepository = iProductSimilarRepository;
        _iOrderService = iOrderService;
        _iOrdersAddressRepository = iOrdersAddressRepository;
        _iOrderProductRepository = iOrderProductRepository;
        _iCustomerNotificationService = iCustomerNotificationService;
        _iWhTransactionRepository = iWhTransactionRepository;
        _iKiotVietService = iKiotVietService;
        _iHostingEnvironment = iHostingEnvironment;
    }

    // GET
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Index(string txtSearch, string startDate, string endDate, int? status, int? payment, 
        int? ship,int? typePayment, int export = 0, int pageindex = 1)
    {
        if (export == 1)
        {
            return RedirectToAction("Export", new {txtSearch,startDate,endDate,status,payment,ship });
        }
        var query = _iOrderServer.GetOrderAll();
        if (!txtSearch.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Code, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Phone, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.Customer.FullName, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Name, "%" + txtSearch.Trim() + "%"));
        }

        if (!string.IsNullOrEmpty(startDate))
        {
            var start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.OrderAt >= start);
        }

        if (!string.IsNullOrEmpty(endDate))
        {
            var end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.OrderAt <= end);
        }

   
     
        if (status != null)
        {
            query = query.Where(x => x.Status == status);
        }

        if (ship.HasValue)
        {
            query = query.Where(x => x.ShipPartner == ship);
        }

        if (payment.HasValue)
        {
            query = payment == 0
                ? query.Where(x => x.StatusPayment == payment || !x.StatusPayment.HasValue)
                : query.Where(x => x.StatusPayment == payment);
        }
        
        if (typePayment.HasValue)
        {
            query = query.Where(x => x.PaymentType == typePayment);
        }

        var listData = PagingList.Create(query.OrderByDescending(x => x.OrderAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            {"txtSearch", txtSearch},
            {"startDate", startDate},
            {"endDate", endDate},
            {"status", status},
            {"payment", payment},
            {"ship", ship},
            { "typePayment", typePayment },
          
        };
        ModelCollection model = new ModelCollection();
        model.AddModel("Title", "T???t c??? ????n h??ng");
        model.AddModel("ListData", listData);
        model.AddModel("ShowOrderStatus", true);
        model.AddModel("StatusCancel", false);
        model.AddModel("Page", pageindex);
        model.AddModel("IsEdit",
            User.HasClaim(CmsClaimType.AreaControllerAction, "Orders@OrderController@Create".ToUpper()));
        model.AddModel("ListStatus", OrderStatusConst.ListStatus);
        return View("Index", model);
    }

    [NonLoad]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Orders@OrderController@Index")]
    public IActionResult IndexOrderCustomerSuccess(string txtSearch, string startDate, string endDate, int? payment,
        int? ship,int? typePayment, int export = 0,int pageindex = 1)
    {
        var query = _iOrderServer.GetOrderAll();
        if (!txtSearch.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Code, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Phone, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.Customer.FullName, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Name, "%" + txtSearch.Trim() + "%"));
        }

        if (!string.IsNullOrEmpty(startDate))
        {
            var start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.OrderAt >= start);
        }

        if (!string.IsNullOrEmpty(endDate))
        {
            var end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.OrderAt <= end);
        }

        if (ship.HasValue)
        {
            query = query.Where(x => x.ShipPartner == ship);
        }

        if (payment.HasValue)
        {
            query = payment == 0
                ? query.Where(x => x.StatusPayment == payment || !x.StatusPayment.HasValue)
                : query.Where(x => x.StatusPayment == payment);
        }
        if (typePayment.HasValue)
        {
            query = query.Where(x => x.PaymentType == typePayment);
        }

        query = query.Where(x => (x.Status == OrderStatusConst.StatusCustomerSuccess)
                                 || (x.Status == OrderStatusConst.StatusWaitCustomerConfirm)
                                 || (x.Status == OrderStatusConst.StatusOrderConfirm));
        var listData = PagingList.Create(query.OrderByDescending(x => x.OrderAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            {"txtSearch", txtSearch},
            {"startDate", startDate},
            {"endDate", endDate},
            {"payment", payment},
            {"ship", ship},
            { "typePayment", typePayment },
        };
        ModelCollection model = new ModelCollection();
        model.AddModel("Title", "Danh s??ch ????n ch??? x??? l??");
        model.AddModel("ListData", listData);
        model.AddModel("ShowOrderStatus", false);
        model.AddModel("Page", pageindex);
        model.AddModel("StatusCancel", false);
        model.AddModel("IsEdit",
            User.HasClaim(CmsClaimType.AreaControllerAction, "Orders@OrderController@Create".ToUpper()));
        model.AddModel("ListStatus", OrderStatusConst.ListStatus);
        return View("Index", model);
    }

    [NonLoad]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Orders@OrderController@Index")]
    public IActionResult IndexOrderCustomerShip(string txtSearch, string startDate, string endDate, int? payment,
        int? ship,int? typePayment, int pageindex = 1)
    {
        var query = _iOrderServer.GetOrderAll();
        if (!txtSearch.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Code, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Phone, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.Customer.FullName, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Name, "%" + txtSearch.Trim() + "%"));
        }

        if (!string.IsNullOrEmpty(startDate))
        {
            var start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.OrderAt >= start);
        }

        if (!string.IsNullOrEmpty(endDate))
        {
            var end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.OrderAt <= end);
        }

        if (ship.HasValue)
        {
            query = query.Where(x => x.ShipPartner == ship);
        }

        if (payment.HasValue)
        {
            query = payment == 0
                ? query.Where(x => x.StatusPayment == payment || !x.StatusPayment.HasValue)
                : query.Where(x => x.StatusPayment == payment);
        }
        if (typePayment.HasValue)
        {
            query = query.Where(x => x.PaymentType == typePayment);
        }

        query = query.Where(x => (x.Status == OrderStatusConst.StatusOrderShip));
        var listData = PagingList.Create(query.OrderByDescending(x => x.OrderAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            {"txtSearch", txtSearch},
            {"startDate", startDate},
            {"endDate", endDate},
            {"payment", payment},
            {"ship", ship},
            { "typePayment", typePayment },
        };
        ModelCollection model = new ModelCollection();
        model.AddModel("Title", "Danh s??ch ????n ??ang giao");
        model.AddModel("ListData", listData);
        model.AddModel("ShowOrderStatus", false);
        model.AddModel("StatusCancel", false);
        model.AddModel("Page", pageindex);
        model.AddModel("IsEdit",
            User.HasClaim(CmsClaimType.AreaControllerAction, "Orders@OrderController@Create".ToUpper()));
        model.AddModel("ListStatus", OrderStatusConst.ListStatus);
        return View("Index", model);
    }

    [NonLoad]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Orders@OrderController@Index")]
    public IActionResult IndexOrderSuccess(string txtSearch, string startDate, string endDate, int? payment, int? ship,
        int? typePayment,int pageindex = 1)
    {
        var query = _iOrderServer.GetOrderAll();
        if (!txtSearch.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Code, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Phone, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.Customer.FullName, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Name, "%" + txtSearch.Trim() + "%"));
        }

        if (!string.IsNullOrEmpty(startDate))
        {
            var start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.OrderAt >= start);
        }

        if (!string.IsNullOrEmpty(endDate))
        {
            var end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.OrderAt <= end);
        }

        if (ship.HasValue)
        {
            query = query.Where(x => x.ShipPartner == ship);
        }

        if (payment.HasValue)
        {
            query = payment == 0
                ? query.Where(x => x.StatusPayment == payment || !x.StatusPayment.HasValue)
                : query.Where(x => x.StatusPayment == payment);
        }
        if (typePayment.HasValue)
        {
            query = query.Where(x => x.PaymentType == typePayment);
        }

        query = query.Where(x => x.Status == OrderStatusConst.StatusOrderSuccess);
        var listData = PagingList.Create(query.OrderByDescending(x => x.OrderAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            {"txtSearch", txtSearch},
            {"startDate", startDate},
            {"endDate", endDate},
            {"status", OrderStatusConst.StatusOrderSuccess},
            {"payment", payment},
            {"ship", ship},
            { "typePayment", typePayment },
        };
        ModelCollection model = new ModelCollection();
        model.AddModel("Title", "Danh s??ch ????n ho??n th??nh");
        model.AddModel("ListData", listData);
        model.AddModel("ShowOrderStatus", false);
        model.AddModel("StatusCancel", false);

        model.AddModel("Page", pageindex);
        model.AddModel("IsEdit",
            User.HasClaim(CmsClaimType.AreaControllerAction, "Orders@OrderController@Create".ToUpper()));
        model.AddModel("ListStatus", OrderStatusConst.ListStatus);
        return View("Index", model);
    }

    [NonLoad]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Orders@OrderController@Index")]
    public IActionResult IndexOrderCancel(string txtSearch, string startDate, string endDate, int? payment, int? ship, int? reasonId,
        int? typePayment,int pageindex = 1)
    {
        var query = _iOrderServer.GetOrderAll();
        if (!txtSearch.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Code, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Phone, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.Customer.FullName, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Name, "%" + txtSearch.Trim() + "%"));
        }

        if (!string.IsNullOrEmpty(startDate))
        {
            var start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.OrderAt >= start);
        }

        if (!string.IsNullOrEmpty(endDate))
        {
            var end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.OrderAt <= end);
        }

        if (ship.HasValue)
        {
            query = query.Where(x => x.ShipPartner == ship);
        }
        if (reasonId.HasValue)
        {
            var note = ReasonCancel.ListReasonCancel.Where(x => x.Type == reasonId).Select(x => x.Name).FirstOrDefault();
            query = query.Where(x => x.ReasonNote == note);
        }
        if (payment.HasValue)
        {
            query = payment == 0
                ? query.Where(x => x.StatusPayment == payment || !x.StatusPayment.HasValue)
                : query.Where(x => x.StatusPayment == payment);
        }
        if (typePayment.HasValue)
        {
            query = query.Where(x => x.PaymentType == typePayment);
        }
        query = query.Where(x => x.Status == OrderStatusConst.StatusOrderCancel);
        var listData = PagingList.Create(query.OrderByDescending(x => x.OrderAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            {"txtSearch", txtSearch},
            {"startDate", startDate},
            {"endDate", endDate},
            {"payment", payment},
            {"ship", ship},
            { "typePayment", typePayment },
            { "reasonId", reasonId },
        };
        ModelCollection model = new ModelCollection();
        model.AddModel("Title", "Danh s??ch ????n h???y");
        model.AddModel("ListData", listData);
        model.AddModel("ShowOrderStatus", false);
        model.AddModel("StatusCancel", true);
        model.AddModel("Page", pageindex);
        model.AddModel("IsEdit",
            User.HasClaim(CmsClaimType.AreaControllerAction, "Orders@OrderController@Create".ToUpper()));
        model.AddModel("ListStatus", OrderStatusConst.ListStatus);
        return View("Index", model);
    }

    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [NonLoad]
    public IActionResult GetProductCartList([FromBody] ProductSimilarModel data)
    {
        try
        {
            if (data.Id == 0)
            {
                return Json(new
                {
                    code = 400,
                    msg = "not found",
                    content = ""
                });
            }

            var rs = _iOrderServer.GetValueCart(data.Id, data.QuantityBy, data.Order, data.Price);
            return Json(new
            {
                code = 200,
                content = rs,
                msg = "L???y d/s s???n ph???m th??nh c??ng"
            });
        }
        catch (Exception ex)
        {
            this._iLogger.LogError($"L???y d/s s???n ph???m l???i , UserId: {UserInfo.UserId} . {ex.Message}");
            return Json(new
            {
                code = 500,
                msg = "fail",
                content = ""
            });
        }
    }

    [NonLoad]
    public IActionResult CheckCoupon(string code, int customerId)
    {
        try
        {
            CustomerCoupon? coupon = _iCustomerCouponRepository.FindByCustomerId(code, customerId);
            if (coupon == null)
            {
                _iLogger.LogError("coupon not found");
                return Json(new
                {
                    code = 404,
                    msg = "not found",
                    content = ""
                });
            }

            return Json(new
            {
                code = 200,
                content = coupon,
                msg = "L???y coupon s???n ph???m th??nh c??ng"
            });
        }
        catch (Exception e)
        {
            _iLogger.LogError(e.Message);
            this._iLogger.LogError($"L???y coupon s???n ph???m  l???i , UserId: {UserInfo.UserId} . {e.Message}");
            return Json(new
            {
                code = 400,
                msg = "fail",
                content = ""
            });
        }
    }

    [NonLoad]
    public IActionResult GetAddressCustomerDefault(int customerId)
    {
        var rs = _iCustomerAddressRepository.FindAll()
            .FirstOrDefault(x => x.CustomerId == customerId && x.IsDefault.Value);
        return Json(new
        {
            code = 200,
            content = rs,
            msg = "L???y d/s GetAddressCustomerDefault th??nh c??ng"
        });
    }

    [HttpGet]
    [NonLoad]
    public IActionResult GetListCustomer()
    {
        var rs = _iCustomerRepository.FindAll().Where(x => x.Status == 1).Select(x => new
        {
            Id = x.Id,
            FullName = $"{x.UserName} - {x.FullName}",
            Type = x.Type,
            TypeGroup = x.TypeGroup,
            x.Email,
            Org = x.Org,
            x.Phone
        }).ToList();
        return Json(new
        {
            code = 200,
            content = rs,
            msg = "L???y d/s kh??ch h??ng th??nh c??ng"
        });
    }

    [HttpPost]
    public IActionResult Create([FromForm] CreateOrderModel model)
    {
        if (!ModelState.IsValid)
        {
            _iLogger.LogError("Create failure IsValid");
            return Ok(new OutputObject(400, "", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(x => x.ErrorMessage).Distinct()
                    .FirstOrDefault())
                .Show());
        }

        try
        {
            List<OrderProduct> orderProducts =
                _iProductRepository.OrderProductsByIds(model.Products!.Select(x => x.ProductSimilarId).ToList());

            if (model.Products == null || orderProducts.Count == 0)
            {
                return Json(new
                {
                    code = 400,
                    msg = "Kh??ng c?? s???n ph???m trong ????n h??ng",
                    content = ""
                });
            }

            // t???o ?????a ch???
            var orderAddress = _iOrderServer.CreateOrderAddress(model.CustomerId.Value, model);
            if (orderAddress == null)
            {
                return Ok(new OutputObject(400, new { }, "Kh??ng t??m th???y ?????a ch??? giao h??ng vui l??ng th??? l???i!").Show());
            }

            // string prFileUrl = "";
            // if (model.PrFile != null)
            // {
            //      prFileUrl = _fileService.SavingFileP(model.PrFile);
            //
            // }
            var orderProduct = model.Products.Join(
                orderProducts,
                viewmodel => viewmodel.ProductSimilarId,
                product => product.ProductSimilarId,
                (viewmodel, product) =>
                {
                    product.Quantity = viewmodel.Quantity;
                    return product;
                }
            ).ToList();
            var customer = _iCustomerRepository.FindById(model!.CustomerId ?? 0);
            var orders = new CMS_EF.Models.Orders.Orders
            {
                CustomerId = model.CustomerId ?? 0,
                PriceShip =  Math.Round( model.PriceShip ?? 0, 0),
                PriceShipSalePercent = model.Percent,
                PriceShipNonSale =  model.PriceNoSale,
                PaymentType = model.PaymentType,
                ShipPartner = model.ShipPartner,
                Total = model.Total,
                ShipType = model.ShipType,
                OrderAt = DateTime.Now,
                LastModifiedAt = DateTime.Now,
                AddressType = model.AddressType,
                CouponCode = model.CouponCode,
                Price = orderProduct.Sum(x => (x.Price ?? 0) * x.Quantity),
                CouponDiscount = model.CouponDiscount,
                Point = 0,
                PrCode = model.PrCode,
                PrFile = model.PrFile,
                Status = 0,
                PointDiscount = 0,
                TotalWeight = model.TotalWeight!.Value,
                BillCompanyName =
                    customer.TypeGroup == 2 ? PrudentialBillInfo.BillCompanyName : model.BillCompanyName,
                BillTaxCode = customer.TypeGroup == 2 ? PrudentialBillInfo.BillTaxCode : model.BillTaxCode,
                BillAddress = customer.TypeGroup == 2 ? PrudentialBillInfo.BillAddress : model.BillAddress,
                BillEmail = customer.TypeGroup == 2 ? PrudentialBillInfo.BillEmail : model.BillEmail,
                OrderProduct = orderProduct,
                OrderAddress = orderAddress
            };
            ResultJson resultJson = _iOrderServer.CreateOrders(orders);
            if (resultJson.StatusCode == 400)
            {
                ILoggingService.Error(this._iLogger, "T???o ????n h??ng l???i , message: " + resultJson.Message,
                    "UserId: " + UserInfo.UserId);

                return Json(new
                {
                    code = 400,
                    content = resultJson.Message,
                    msg = "fail"
                });
            }

            ToastMessage(1, "T???o ????n h??ng th??nh c??ng !");
            ILoggingService.Infor(this._iLogger, "T???o ????n h??ng th??nh c??ng , Id: " + orders.Id,
                "UserId: " + UserInfo.UserId);

            return Json(new
            {
                code = 200,
                content = resultJson.Data,
                msg = "T???o order th??nh c??ng"
            });
        }
        catch (Exception ex)
        {
            ILoggingService.Error(this._iLogger, "T???o ????n h??ng th??nh c??ng  ", "UserId: " + UserInfo.UserId, ex);

            return Json(new
            {
                code = 400,
                msg = "fail",
                content = ex.Message
            });
        }
    }


    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Details(string id)
    {
        try
        {
            var order = _iOrderServer.GetOrder(id);
            DetailsViewModel model = new DetailsViewModel();
            string ship = ShipConst.GetShipment(order.ShipPartner!.Value, order.ShipType);
            model.Order = order;
            model.Ship = ship;
            var adress = _iOrderServer.GetAddress(order.OrderAddress);
            string pttt = "";
            if (order.PaymentType == 0)
            {
                pttt = "Thanh to??n khi nh???n h??ng";
            }
            else if (order.PaymentType == 1)
            {
                pttt = "Th??? t??n d???ng";
            }
            else if (order.PaymentType == 2)
            {
                pttt = "Th??? ATM n???i ?????a (Internet  banking) ";
            }

            model.ListOrderPartnerShipLog = new List<OrderPartnerShipLog>();
            model.ListOrderPartnerShipLog = this._iOrderService.GetOrderPartnerShipLogByCode(order.Code);
            model.IsPoi = IsPoi;
            model.Payment = pttt;
            model.Address = adress;
            model.IsEdit = (order.Status == OrderStatusConst.StatusCustomerSuccess) ||
                           (order.Status == OrderStatusConst.StatusWaitCustomerConfirm);
            model.IsOrderConfirm =
                User.HasClaim(CmsClaimType.AreaControllerAction,
                    "Orders@OrderController@ChangeOrderConfirm".ToUpper()) &&
                (order.Status != OrderStatusConst.StatusOrderSuccess &&
                 order.Status != OrderStatusConst.StatusOrderCancel) &&
                (order.Status == OrderStatusConst.StatusWaitCustomerConfirm ||
                 order.Status == OrderStatusConst.StatusCustomerSuccess);
            model.IsOrderShip =
                User.HasClaim(CmsClaimType.AreaControllerAction, "Orders@OrderController@ChangeOrderShip".ToUpper()) &&
                (order.Status == OrderStatusConst.StatusWaitCustomerConfirm ||
                 order.Status == OrderStatusConst.StatusCustomerSuccess ||
                 order.Status == OrderStatusConst.StatusOrderConfirm);
            model.IsOrderSuccess =
                User.HasClaim(CmsClaimType.AreaControllerAction,
                    "Orders@OrderController@ChangeOrderSuccess".ToUpper()) &&
                (order.Status != OrderStatusConst.StatusOrderSuccess) &&
                (order.Status != OrderStatusConst.StatusOrderCancel);
            model.IsOrderCancel =
                User.HasClaim(CmsClaimType.AreaControllerAction,
                    "Orders@OrderController@ChangeOrderCancel".ToUpper()) &&
                (order.Status != OrderStatusConst.StatusOrderSuccess) &&
                (order.Status != OrderStatusConst.StatusOrderCancel);
            model.IsStatusPayment =
                User.HasClaim(CmsClaimType.AreaControllerAction, "Orders@OrderController@StatusPayment".ToUpper()) &&
                (order.Status != OrderStatusConst.StatusOrderCancel);
            model.IsStatusShowAll =
                model.IsOrderConfirm || model.IsOrderShip || model.IsOrderSuccess || model.IsOrderCancel;
            model.IsStatusSynchronizedKiot =
                User.HasClaim(CmsClaimType.AreaControllerAction,
                    "Orders@OrderController@ChangeOrderSynchronizedKiot".ToUpper()) && (order.OrderIdWh == null);
            model.IsPoi = IsPoi;
            return View(model);
        }
        catch (Exception e)
        {
            _iLogger.LogError(e, $"Details: code: {id}");
        }

        return NotFound();
    }

    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Edit(string id)
    {
        ModelCollection model = new ModelCollection();
        var order = _iOrdersRepository.FindAll().FirstOrDefault(x => x.Code == id);
        if (order == null)
        {
            ToastMessage(-1, "D??? li???u code kh??ng t???n t???i");
            model.AddModel("id", 0);
        }

        if (!(order?.Status == OrderStatusConst.StatusWaitCustomerConfirm ||
              order?.Status == OrderStatusConst.StatusCustomerSuccess))
        {
            return NotFound();
        }
        else
        {
            model.AddModel("id", order.Id);
        }

        return View(model);
    }

    [NonLoad]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Orders@OrderController@Index")]
    public IActionResult Export(string txtSearch, string startDate, string endDate, int? status, int? payment, int? ship)
    {
        var query = _iOrderServer.GetOrderAll();
        if (!txtSearch.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Code, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Phone, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.Customer.FullName, "%" + txtSearch.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Name, "%" + txtSearch.Trim() + "%"));
        }

        if (!string.IsNullOrEmpty(startDate))
        {
            var start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.OrderAt >= start);
        }

        if (!string.IsNullOrEmpty(endDate))
        {
            var end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.OrderAt <= end);
        }

        if (status != null)
        {
            query = query.Where(x => x.Status == status);
        }

        if (ship.HasValue)
        {
            query = query.Where(x => x.ShipPartner == ship);
        }

        if (payment.HasValue)
        {
            query = payment == 0
                ? query.Where(x => x.StatusPayment == payment || !x.StatusPayment.HasValue)
                : query.Where(x => x.StatusPayment == payment);
        }

        var listData = query.Select( x => new ExportForControlViewModel
        {
            Code = x.Code,
            Status = x.StatusPayment,
            StatusStr = x.StatusPayment == ExcelStatus.Paid.Status ? ExcelStatus.Paid.StatusStr : ExcelStatus.Unpaid.StatusStr
        }).ToList();

        var filePath = Path.Combine(_iHostingEnvironment.WebRootPath, "Templates/Excels/Orders", "doi_soat_thanh_toan.xlsx");
        var template = new XLTemplate(filePath);

        template.AddVariable("ListData", listData);
        template.Generate();
        byte[] excelFile;
        using (var ms = new MemoryStream())
        {
            template.SaveAs(ms);
            ms.Position = 0;
            excelFile = ms.ToArray();
        }

        ILoggingService.Infor(_iLogger, "Xu???t file ?????i so??t ????n h??ng", "Th??nh c??ng");
        return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Doi_soat_don_dat_hang.xlsx");
    }

    [NonLoad]
    [HttpGet]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Orders@OrderController@Edit")]
    public IActionResult UpFile()
    {
        var model = new UpFileViewModel();
        return View(model);
    } 

    [NonLoad]
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Orders@OrderController@Edit")]
    public IActionResult UpFile([FromForm] UpFileViewModel form)
    {
        if (!ModelState.IsValid) return View(form);
        try
        {
            var file = form.File;
            var dataFile = _iOrderService.ReadDataFromExcelAndValidate(file);
            _iOrderService.ImportData(dataFile);
            ToastMessage(1, "Nh???p d??? li???u th??nh c??ng");
            ILoggingService.Infor(_iLogger, "UpFile", "Nh???p file ?????i so??t s???n ph???m th??nh c??ng");
        }
        catch (Exception e)
        {
            ToastMessage(-1, "Nh???p d??? li???u l???i, " + e.Message);
            ILoggingService.Error(_iLogger, "UpFile", "Nh???p file ?????i so??t s???n ph???m l???i, " + e.Message);
        }

        return View(form);
    }
    
    [HttpPost]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult ChangeOrderConfirm([FromBody] JObject data)
    {
        try
        {
            string id = $"{data["id"]}";
            if (!string.IsNullOrEmpty(id))
            {
                var order = this._iOrdersRepository.FindByCode(id);
                if (order != null)
                {
                    if (order.Status == OrderStatusConst.StatusWaitCustomerConfirm ||
                        order.Status == OrderStatusConst.StatusCustomerSuccess)
                    {
                        var rs = _iOrderService.UpdateOrderStatus(order, OrderStatusConst.StatusOrderConfirm,
                            UserInfo);
                        if (rs)
                        {
                            if (order.CustomerId.HasValue)
                            {
                                this._iCustomerNotificationService.SendCustomerNotification(order.CustomerId.Value,
                                    new CustomerNotificationObject()
                                    {
                                        Title = $"{AppConst.AppName} ???? x??c nh???n ????n h??ng {order.Code}",
                                        Detail = "",
                                        Link = $"/account/purchase/{order.Code}"
                                    });
                            }

                            ToastMessage(1, "C???p nh???t tr???ng th??i ????n h??ng th??nh c??ng");
                            return Ok(new OutputObject(200, "", $"C???p nh???t tr???ng th??i ????n h??ng th??nh c??ng").Show());
                        }
                        else
                        {
                            return Ok(new OutputObject(404, "", $"C???p nh???t tr???ng th??i ????n h??ng l???i").Show());
                        }
                    }
                    else
                    {
                        return Ok(new OutputObject(404, "",
                                $"Tr???ng th??i ????n h??ng ??ang l?? {OrderStatusConst.ListStatus.FirstOrDefault(x => x.Key == order.Status)!.Value}, b???n kh??ng th??? c???p nh???t")
                            .Show());
                    }
                }
            }

            return Ok(new OutputObject(404, "", "????n h??ng kh??ng t???n t???i").Show());
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"M?? order {data}");
            return Ok(new OutputObject(500, "", "????n h??ng kh??ng t???n t???i").Show());
        }
    }

    [HttpPost]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult ChangeOrderShip([FromBody] JObject data)
    {
        try
        {
            string id = $"{data["id"]}";
            int? cod = CmsFunction.ConvertToInt($"{data["cod"] ?? "0"}");
            if (!string.IsNullOrEmpty(id))
            {
                var order = this._iOrdersRepository.FindByCodeIncludeProduct(id.ToUpper());
                if (order != null)
                {
                    if (order.Status == OrderStatusConst.StatusWaitCustomerConfirm ||
                        order.Status == OrderStatusConst.StatusCustomerSuccess ||
                        order.Status == OrderStatusConst.StatusOrderConfirm)
                    {
                        if (order.PaymentType == PaymentMethodConst.Debit.Status && order.StatusPayment == OrderStatusPayment.StatusNoPayment)
                        {
                            return Ok(new OutputObject(404, "", $"????n h??ng {order.Code} thanh to??n chuy???n kho???n, vui l??ng x??c nh???n thanh to??n tr?????c khi g???i v???n chuy???n")
                                .Show());
                        }
                        bool isShip = false;
                        if ((order.ShipPartner == 1 || order.ShipPartner == 2))
                        {
                            if (order.Total > 5000000)
                            {
                                order.CodAmountEvaluation = cod;
                            }
                            else
                            {
                                order.CodAmountEvaluation = order.Total;
                            }

                            if (order.PaymentType == PaymentMethodConst.Debit.Status)
                            {
                                order.CodAmountEvaluation = 0;
                            }

                            var ship = this._iOrderService.SendShip(order);
                            if (ship != null && !string.IsNullOrEmpty(ship.OrderCode))
                            {
                                order.CodeShip = ship.OrderCode;
                                order.ShipCodeIdVnPost = ship.Id;
                                isShip = true;
                            }
                            else
                            {
                                return Ok(new OutputObject(404, "",
                                        $"{(!string.IsNullOrEmpty(ship?.Err) ? ship?.Err : "C???p nh???t tr???ng th??i ????n h??ng g???i v???n chuy???n l???i")}")
                                    .Show());
                            }
                        }
                        else
                        {
                            isShip = true;
                        }

                        if (isShip)
                        {
                            var rs = this._iOrderService.UpdateOrderStatus(order, OrderStatusConst.StatusOrderShip,
                                UserInfo);
                            if (rs)
                            {
                                ToastMessage(1, "C???p nh???t tr???ng th??i ????n h??ng th??nh c??ng");
                                return Ok(new OutputObject(200, "", $"G???i ????n h??ng th??nh c??ng").Show());
                            }
                            else
                            {
                                return Ok(new OutputObject(404, "", $"C???p nh???t tr???ng th??i ????n h??ng g???i v???n chuy???n l???i")
                                    .Show());
                            }
                        }
                        else
                        {
                            return Ok(
                                new OutputObject(404, "", $"C???p nh???t tr???ng th??i ????n h??ng g???i v???n chuy???n l???i").Show());
                        }
                    }
                    else
                    {
                        return Ok(new OutputObject(404, "",
                                $"Tr???ng th??i ????n h??ng ??ang l?? {OrderStatusConst.ListStatus.FirstOrDefault(x => x.Key == order.Status)!.Value}, b???n kh??ng th??? c???p nh???t")
                            .Show());
                    }
                }
            }

            return Ok(new OutputObject(404, "", "????n h??ng kh??ng t???n t???i").Show());
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"M?? order {data}");
            return Ok(new OutputObject(500, "", "????n h??ng kh??ng t???n t???i").Show());
        }
    }

    [HttpPost]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult ChangeOrderSuccess([FromBody] JObject data)
    {
        try
        {
            string id = $"{data["id"]}";
            if (!string.IsNullOrEmpty(id))
            {
                var order = this._iOrdersRepository.FindByCode(id);
                if (order != null)
                {
                    var rs = this._iOrderService.UpdateOrderStatus(order, OrderStatusConst.StatusOrderSuccess,
                        UserInfo);
                    if (rs)
                    {
                        if (order.ShipPartner != ShipConst.GHN && order.ShipPartner != ShipConst.VNnPost &&
                            order.CustomerId.HasValue)
                        {
                            this._iCustomerNotificationService.SendCustomerNotification(order.CustomerId.Value,
                                new CustomerNotificationObject()
                                {
                                    Title = $"????n h??ng {order.Code} ???? ???????c giao th??nh c??ng",
                                    Link = $"/account/purchase/{order.Code}",
                                    Detail = $"C???m ??n Qu?? Kh??ch h??ng ???? ?????ng h??nh c??ng {AppConst.AppName}"
                                });
                        }

                        ToastMessage(1, "C???p nh???t tr???ng th??i ????n h??ng th??nh c??ng");
                        return Ok(new OutputObject(200, "", $"C???p nh???t tr???ng th??i ????n h??ng th??nh c??ng").Show());
                    }
                    else
                    {
                        return Ok(new OutputObject(404, "", $"C???p nh???t tr???ng th??i ????n h??ng l???i").Show());
                    }
                }
            }

            return Ok(new OutputObject(404, "", "????n h??ng kh??ng t???n t???i").Show());
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"M?? order {data}");
            return Ok(new OutputObject(500, "", "????n h??ng kh??ng t???n t???i").Show());
        }
    }

    [HttpPost]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult ChangeOrderSynchronizedKiot([FromBody] JObject data)
    {
        try
        {
            string id = $"{data["id"]}";
            if (!string.IsNullOrEmpty(id))
            {
                var order = this._iOrdersRepository.FindByCodeWithProduct(id);
                if (order != null)
                {
                    if (order.OrderIdWh == null)
                    {
                        var o = this._iWhTransactionRepository.FindByOrderIdStatus(order.Id, WhTransactionConst.Create);
                        var wareHouse = this._iKiotVietService.CreateOrder(order);
                        if (wareHouse != null)
                        {
                            if (wareHouse.Status == 1)
                            {
                                this._iLogger.LogInformation(
                                    $"?????ng b??? ????n h??ng {order.Code} sang kiot vi???t th??nh c??ng");
                                order.OrderIdWh = wareHouse.OrderId;
                                this._iOrdersRepository.Update(order);
                                if (o != null)
                                {
                                    this._iWhTransactionRepository.Delete(o);
                                }
                            }
                            else
                            {
                                // x??? l?? l??u db ????? call l???i khi kiot vi???t l???i
                                if (o == null)
                                {
                                    WhTransaction whTransaction = new WhTransaction()
                                    {
                                        OrderId = order.Id,
                                        Status = WhTransactionConst.Create,
                                        CreatedAt = DateTime.Now
                                    };
                                    this._iWhTransactionRepository.Create(whTransaction);
                                }
                                else
                                {
                                    o.CreatedAt = DateTime.Now;
                                    this._iWhTransactionRepository.Update(o);
                                }

                                return Ok(new OutputObject(404, "",
                                        $"{wareHouse.Msg}")
                                    .Show());
                            }
                        }

                        ToastMessage(1, "?????ng b??? ????n h??ng th??nh c??ng");
                        return Ok(new OutputObject(200, "", $"?????ng b??? ????n h??ng th??nh c??ng").Show());
                    }
                    else
                    {
                        return Ok(new OutputObject(404, "",
                                $"????n h??ng ???? c???p nh???t n??n b???n kh??ng th??? ti???p t???c c???p nh???t")
                            .Show());
                    }
                }
            }

            return Ok(new OutputObject(404, "", "????n h??ng kh??ng t???n t???i").Show());
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"M?? order {data}");
            return Ok(new OutputObject(500, "", "????n h??ng kh??ng t???n t???i").Show());
        }
    }


    [HttpPost]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult ChangeOrderCancel([FromBody] OrderCancelRequest data)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var note = ReasonCancel.ListReasonCancel.Where(x => x.Type == data.Note).Select(x => x.Name).FirstOrDefault();
                var rs = this._iOrderService.CancelOrders(data.Id, note);
                if (rs.StatusCode == 200)
                {
                    ToastMessage(1, $"H???y ????n h??ng {data.Id} th??nh c??ng");
                }

                return Ok(rs);
            }
            else
            {
                return Ok(new OutputObject(400, "", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(x => x.ErrorMessage)
                    .Distinct().FirstOrDefault()).Show());
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"M?? order {data}");
            return Ok(new OutputObject(500, "", "????n h??ng kh??ng t???n t???i").Show());
        }
    }
    
    [HttpGet]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Orders@OrderController@ChangeOrderCancel")]
    public IActionResult GetReasonOrderCancel()
    {
        var rs = ReasonCancel.ListReasonCancel;
        return Json(new
        {
            code = 200,
            content = rs,
            msg = "L???y d/s s???n ph???m th??nh c??ng"
        });
    }
    [HttpPost]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult StatusPayment([FromBody] StatusPaymentModel data)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var o = this._iOrdersRepository.FindByCode(data.OrderCode);
                if (o == null)
                {
                    return Ok(new OutputObject(500, "", "????n h??ng kh??ng t???n t???i").Show());
                }

                o.StatusPayment = data.Status;
                this._iOrdersRepository.Update(o);
                ToastMessage(1, $"C???p nh???t tr???ng th??i thanh to??n ????n h??ng {data.OrderCode} th??nh c??ng");
                return Ok(new OutputObject(200, "",
                    $"C???p nh???t tr???ng th??i thanh toa ????n h??ng {data.OrderCode} th??nh c??ng").Show());
            }
            else
            {
                return Ok(new OutputObject(400, "", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(x => x.ErrorMessage)
                    .Distinct().FirstOrDefault()).Show());
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"{UserInfo.UserId} c??p nh???t tr???ng th??i thanh to??n m?? order {data.OrderCode}");
            return Ok(new OutputObject(500, "", "????n h??ng kh??ng t???n t???i").Show());
        }
    }

    [NonLoad]
    public IActionResult GetOrderEdit(int id)
    {
        try
        {
            var rs = _iOrderServer.GetOrderById(id);
            return Json(new
            {
                code = 200,
                content = rs,
                msg = "L???y GetOrderEdit th??nh c??ng"
            });
        }
        catch (Exception e)
        {
            _iLogger.LogError(e, $"Details: code: {id}");
        }

        return Json(new
        {
            code = 400,
            content = "",
            msg = "L???y GetOrderEdit l???i"
        });
    }

    [NonLoad]
    public IActionResult GetListOrderEdit(int id)
    {
        try
        {
            if (id == 0)
            {
                return Json(new
                {
                    code = 400,
                    msg = "not found",
                    content = ""
                });
            }

            var rs = _iOrderServer.GetListOrderProduct(id);
            return Json(new
            {
                code = 200,
                content = rs,
                msg = "L???y d/s s???n ph???m th??nh c??ng"
            });
        }
        catch (Exception ex)
        {
            this._iLogger.LogError($"L???y d/s s???n ph???m l???i , UserId: {UserInfo.UserId} . {ex.Message}");
            return Json(new
            {
                code = 500,
                msg = "fail",
                content = ""
            });
        }
    }

    [NonLoad]
    public IActionResult GetCustomer(int id)
    {
        try
        {
            var rs = _iCustomerRepository.FindById(id);
            if (rs == null)
            {
                return Json(new
                {
                    code = 400,
                    content = "",
                    msg = "L???y GetCustomer l???i"
                });
            }

            rs.FullName = $"{rs.UserName} - {rs.FullName}";
            return Json(new
            {
                code = 200,
                content = rs,
                msg = "L???y GetCustomer th??nh c??ng"
            });
        }
        catch (Exception e)
        {
            _iLogger.LogError(e, $"GetCustomer: code: {id}");
        }

        return Json(new
        {
            code = 400,
            content = "",
            msg = "L???y GetCustomer l???i"
        });
    }


    [NonLoad]
    public IActionResult GetPriceProduct(int id)
    {
        try
        {
            var rs = _iOrderServer.GetPriceWeight(id);
            return Json(new
            {
                code = 200,
                content = rs,
                msg = "L???y GetPriceProduct th??nh c??ng"
            });
        }
        catch (Exception e)
        {
            _iLogger.LogError(e, $"GetCustomer: code: {id}");
        }

        return Json(new
        {
            code = 400,
            content = "",
            msg = "L???y GetCustomer l???i"
        });
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public IActionResult Edit([FromForm] EditOrderModel model)
    {
        if (!ModelState.IsValid)
        {
            _iLogger.LogError("Edit failure IsValid");
            return Ok(new OutputObject(400, "", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(x => x.ErrorMessage).Distinct()
                    .FirstOrDefault())
                .Show());
        }

        try
        {
            var modelApi = new OrderEditApiModel();
            bool isChangProduct = false;
            CMS_EF.Models.Orders.Orders order = _iOrdersRepository.FindAll().Where(x => x.Id == model.OrderId)
                .FirstOrDefault();
            List<OrderProduct> orderCreate = new List<OrderProduct>();
            List<OrderProduct> orderUpdate = new List<OrderProduct>();
            List<OrderProduct> orderDelete = new List<OrderProduct>();
            if (model.CheckChangeP)
            {
                List<int> listSimilar = model.Products!.Select(x => x.ProductSimilarId).ToList();
                orderDelete = _iOrderProductRepository.FindAll().Where(x =>
                    x.OrderId == model.OrderId && !listSimilar.Contains(x.ProductSimilarId.Value)).ToList();
                orderUpdate = _iOrderProductRepository.FindAll().Where(x =>
                    x.OrderId == model.OrderId && listSimilar.Contains(x.ProductSimilarId.Value)).ToList();
                List<int> listUpdate = orderUpdate.Select(x => x.ProductSimilarId.Value).ToList();
                List<int> listCreate = listSimilar.Where(x => !listUpdate.Contains(x)).ToList();
                orderCreate = _iProductRepository.OrderProductsEditByIds(listCreate, model.OrderId);
                isChangProduct = true;
            }
            else
            {
                orderUpdate = _iOrderProductRepository.FindAll().Where(x => x.OrderId == model.OrderId).ToList();
            }

            if (model.Products == null || (orderUpdate.Count == 0 && orderCreate.Count == 0))
            {
                return Json(new
                {
                    code = 400,
                    msg = "Kh??ng c?? s???n ph???m trong ????n h??ng",
                    content = ""
                });
            }

            // t???o ?????a ch???
            var newAddress = new OrderAddress();

            newAddress.Address = model.Address;
            newAddress.Name = model.Name;
            newAddress.Email = model.Email;
            newAddress.Note = model.Note ?? "";
            newAddress.Phone = model.Phone;
            newAddress.CommuneCode = model.CommuneCode;
            newAddress.DistrictCode = model.DistrictCode;
            newAddress.ProvinceCode = model.ProvinceCode;


            string prFileUrl = "";
            if (model.PrFile != null)
            {
                prFileUrl = _fileService.SavingFileP(model.PrFile);
            }

            modelApi.ListOrderProductCreate = model.Products.Join(
                orderCreate,
                viewmodel => viewmodel.ProductSimilarId,
                product => product.ProductSimilarId,
                (viewmodel, product) =>
                {
                    if (product.Quantity != viewmodel.Quantity)
                    {
                        isChangProduct = true;
                    }

                    product.Quantity = viewmodel.Quantity;
                    return product;
                }
            ).ToList();
            modelApi.ListOrderProductUpdate = model.Products.Join(
                orderUpdate,
                viewmodel => viewmodel.ProductSimilarId,
                product => product.ProductSimilarId,
                (viewmodel, product) =>
                {
                    if (product.Quantity != viewmodel.Quantity)
                    {
                        isChangProduct = true;
                    }

                    product.Quantity = viewmodel.Quantity;
                    return product;
                }
            ).ToList();
            modelApi.ListOrderProductDelete = orderDelete;
            var customer = _iCustomerRepository.FindById(model!.CustomerId ?? 0);
            if (model.PriceShip != order.PriceShip)
            {
                order.PriceShipNonSale = null;
                order.PriceShipSalePercent = null;

            }
            order.PriceShip = model.PriceShip;
            order.PaymentType = model.PaymentType;
            order.ShipPartner = model.ShipPartner;
            order.Total = model.Total;
            order.ShipType = model.ShipType;
            order.AddressType = model.AddressType;
            order.CouponCode = model.CouponCode;
            order.CouponDiscount = model.CouponDiscount;
            order.Price = orderCreate.Sum(x => (x.Price ?? 0) * x.Quantity) +
                          orderUpdate.Sum(x => (x.Price ?? 0) * x.Quantity);
            // order.Point = order.Point;
            order.PrCode = model.PrCode;
            // order.PointDiscount = model.Point * IsPoi;
            order.PrFile = !string.IsNullOrEmpty(prFileUrl) ? prFileUrl : order.PrFile;
            order.BillCompanyName =
                customer.TypeGroup == 2 ? PrudentialBillInfo.BillCompanyName : model.BillCompanyName;
            order.BillTaxCode = customer.TypeGroup == 2 ? PrudentialBillInfo.BillTaxCode : model.BillTaxCode;
            order.BillAddress = customer.TypeGroup == 2 ? PrudentialBillInfo.BillAddress : model.BillAddress;
            order.BillEmail = customer.TypeGroup == 2 ? PrudentialBillInfo.BillEmail : model.BillEmail;
            modelApi.Order = order;
            modelApi.OrderAddress = newAddress;
            modelApi.IsChangeProduct = isChangProduct;
            // modelApi.IsChangePoi = model.CheckChangePoi;
            ResultJson resultJson = _iOrderServer.EditOrders(modelApi);
            if (resultJson.StatusCode == 400)
            {
                ILoggingService.Error(this._iLogger, "S???a ????n h??ng l???i  " + resultJson.Message,
                    "UserId: " + UserInfo.UserId);

                return Json(new
                {
                    code = 400,
                    content = resultJson.Message,
                    msg = "fail"
                });
            }

            ToastMessage(1, "S???a ????n h??ng th??nh c??ng !");
            ILoggingService.Infor(this._iLogger, "S???a ????n h??ng th??nh c??ng Id: " + order.Id,
                "UserId: " + UserInfo.UserId);

            return Json(new
            {
                code = 200,
                content = resultJson.Data,
                msg = "s???a order th??nh c??ng"
            });
        }
        catch (Exception ex)
        {
            ILoggingService.Error(this._iLogger, "S???a ????n h??ng l???i  ", "UserId: " + UserInfo.UserId, ex);

            this._iLogger.LogError($"S???a order l???i , UserId: {UserInfo.UserId} . {ex.Message}");
            return Json(new
            {
                code = 400,
                msg = "fail",
                content = ex.Message
            });
        }
    }
}