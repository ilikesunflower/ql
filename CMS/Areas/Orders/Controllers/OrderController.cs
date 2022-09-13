using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Core.Internal;
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
using CMS.Controllers;
using CMS.Models;
using CMS.Models.ModelContainner;
using CMS.Services.Files;
using Microsoft.AspNetCore.Authorization;
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

    private double IsPoi = PointConst.Coefficient;

    public OrderController(ILogger<OrderController> iLogger, IOrderServer iOrderServer,
        ICustomerCouponRepository iCustomerCouponRepository,
        ICustomerRepository iCustomerRepository, ICustomerAddressRepository iCustomerAddressRepository,
        IProductRepository iProductRepository,
        IFileService fileService, IOrdersRepository iOrdersRepository,
        IProductSimilarRepository iProductSimilarRepository, IOrderService iOrderService,
        IOrdersAddressRepository iOrdersAddressRepository, IOrderProductRepository iOrderProductRepository,
        ICustomerNotificationService iCustomerNotificationService)
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
    }

    // GET
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Index(string txtKeyword, string startDate, string endDate, int? status, int? payment,
        int? ship, int pageindex = 1)
    {
        var query = _iOrderServer.GetOrderAll();
        if (!txtKeyword.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Code, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Phone, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.Customer.FullName, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Name, "%" + txtKeyword.Trim() + "%"));
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

        var listData = PagingList.Create(query.OrderByDescending(x => x.OrderAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            { "txtKeyword", txtKeyword },
            { "startDate", startDate },
            { "endDate", endDate },
            { "status", status },
            { "payment", payment },
            { "ship", ship },
        };
        ModelCollection model = new ModelCollection();
        model.AddModel("Title", "Tất cả đơn hàng");
        model.AddModel("ListData", listData);
        model.AddModel("ShowOrderStatus", true);
        model.AddModel("Page", pageindex);
        model.AddModel("IsEdit",
            User.HasClaim(CmsClaimType.AreaControllerAction, "Orders@OrderController@Create".ToUpper()));
        model.AddModel("ListStatus", OrderStatusConst.ListStatus);
        return View("Index", model);
    }

    [NonLoad]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Orders@OrderController@Index")]
    public IActionResult IndexOrderCustomerSuccess(string txtKeyword, string startDate, string endDate, int? payment,
        int? ship, int pageindex = 1)
    {
        var query = _iOrderServer.GetOrderAll();
        if (!txtKeyword.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Code, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Phone, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.Customer.FullName, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Name, "%" + txtKeyword.Trim() + "%"));
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

        query = query.Where(x => (x.Status == OrderStatusConst.StatusCustomerSuccess)
                                 || (x.Status == OrderStatusConst.StatusWaitCustomerConfirm)
                                 || (x.Status == OrderStatusConst.StatusOrderConfirm));
        var listData = PagingList.Create(query.OrderByDescending(x => x.OrderAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            { "txtKeyword", txtKeyword },
            { "startDate", startDate },
            { "endDate", endDate },
            { "payment", payment },
            { "ship", ship },
        };
        ModelCollection model = new ModelCollection();
        model.AddModel("Title", "Danh sách đơn chờ xử lý");
        model.AddModel("ListData", listData);
        model.AddModel("ShowOrderStatus", false);
        model.AddModel("Page", pageindex);
        model.AddModel("IsEdit",
            User.HasClaim(CmsClaimType.AreaControllerAction, "Orders@OrderController@Create".ToUpper()));
        model.AddModel("ListStatus", OrderStatusConst.ListStatus);
        return View("Index", model);
    }

    [NonLoad]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Orders@OrderController@Index")]
    public IActionResult IndexOrderCustomerShip(string txtKeyword, string startDate, string endDate, int? payment,
        int? ship, int pageindex = 1)
    {
        var query = _iOrderServer.GetOrderAll();
        if (!txtKeyword.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Code, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Phone, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.Customer.FullName, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Name, "%" + txtKeyword.Trim() + "%"));
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

        query = query.Where(x => (x.Status == OrderStatusConst.StatusOrderShip));
        var listData = PagingList.Create(query.OrderByDescending(x => x.OrderAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            { "txtKeyword", txtKeyword },
            { "startDate", startDate },
            { "endDate", endDate },
            { "payment", payment },
            { "ship", ship },
        };
        ModelCollection model = new ModelCollection();
        model.AddModel("Title", "Danh sách đơn đang giao");
        model.AddModel("ListData", listData);
        model.AddModel("ShowOrderStatus", false);
        model.AddModel("Page", pageindex);
        model.AddModel("IsEdit",
            User.HasClaim(CmsClaimType.AreaControllerAction, "Orders@OrderController@Create".ToUpper()));
        model.AddModel("ListStatus", OrderStatusConst.ListStatus);
        return View("Index", model);
    }

    [NonLoad]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Orders@OrderController@Index")]
    public IActionResult IndexOrderSuccess(string txtKeyword, string startDate, string endDate, int? payment, int? ship,
        int pageindex = 1)
    {
        var query = _iOrderServer.GetOrderAll();
        if (!txtKeyword.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Code, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Phone, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.Customer.FullName, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Name, "%" + txtKeyword.Trim() + "%"));
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

        query = query.Where(x => x.Status == OrderStatusConst.StatusOrderSuccess);
        var listData = PagingList.Create(query.OrderByDescending(x => x.OrderAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            { "txtKeyword", txtKeyword },
            { "startDate", startDate },
            { "endDate", endDate },
            { "status", OrderStatusConst.StatusOrderSuccess },
            { "payment", payment },
            { "ship", ship },
        };
        ModelCollection model = new ModelCollection();
        model.AddModel("Title", "Danh sách đơn hoàn thành");
        model.AddModel("ListData", listData);
        model.AddModel("ShowOrderStatus", false);
        model.AddModel("Page", pageindex);
        model.AddModel("IsEdit",
            User.HasClaim(CmsClaimType.AreaControllerAction, "Orders@OrderController@Create".ToUpper()));
        model.AddModel("ListStatus", OrderStatusConst.ListStatus);
        return View("Index", model);
    }

    [NonLoad]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Orders@OrderController@Index")]
    public IActionResult IndexOrderCancel(string txtKeyword, string startDate, string endDate, int? payment, int? ship,
        int pageindex = 1)
    {
        var query = _iOrderServer.GetOrderAll();
        if (!txtKeyword.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Code, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Phone, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.Customer.FullName, "%" + txtKeyword.Trim() + "%") ||
                                     EF.Functions.Like(x.OrderAddress.Name, "%" + txtKeyword.Trim() + "%"));
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

        query = query.Where(x => x.Status == OrderStatusConst.StatusOrderCancel);
        var listData = PagingList.Create(query.OrderByDescending(x => x.OrderAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            { "txtKeyword", txtKeyword },
            { "startDate", startDate },
            { "endDate", endDate },
            { "payment", payment },
            { "ship", ship },
        };
        ModelCollection model = new ModelCollection();
        model.AddModel("Title", "Danh sách đơn hủy");
        model.AddModel("ListData", listData);
        model.AddModel("ShowOrderStatus", false);
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
        int i = 0;
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
                msg = "Lấy d/s sản phẩm thành công"
            });
        }
        catch (Exception ex)
        {
            this._iLogger.LogError($"Lấy d/s sản phẩm lỗi , UserId: {UserInfo.UserId} . {ex.Message}");
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
                msg = "Lấy coupon sản phẩm thành công"
            });
        }
        catch (Exception e)
        {
            _iLogger.LogError(e.Message);
            this._iLogger.LogError($"Lấy coupon sản phẩm  lỗi , UserId: {UserInfo.UserId} . {e.Message}");
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
            msg = "Lấy d/s GetAddressCustomerDefault thành công"
        });
    }

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
            msg = "Lấy d/s khách hàng thành công"
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
                    msg = "Không có sản phẩm trong đơn hàng",
                    content = ""
                });
            }

            // tạo địa chỉ
            var orderAddress = _iOrderServer.CreateOrderAddress(model.CustomerId.Value, model);
            if (orderAddress == null)
            {
                return Ok(new OutputObject(400, new { }, "Không tìm thấy địa chỉ giao hàng vui lòng thử lại!").Show());
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
                PriceShip = model.PriceShip,
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
                Point = model.Point,
                PrCode = model.PrCode,
                PrFile = model.PrFile,
                Status = 0,
                PointDiscount = (double)(model.Point * IsPoi),
                TotalWeight = model.TotalWeight!.Value,
                BillCompanyName =
                    customer.Type == 2 ? PrudentialBillInfo.BillCompanyName : model.BillCompanyName,
                BillTaxCode = customer.Type == 2 ? PrudentialBillInfo.BillTaxCode : model.BillTaxCode,
                BillAddress = customer.Type == 2 ? PrudentialBillInfo.BillAddress : model.BillAddress,
                BillEmail = customer.Type == 2 ? PrudentialBillInfo.BillEmail : model.BillEmail,
                OrderProduct = orderProduct,
                OrderAddress = orderAddress
            };
            ResultJson resultJson = _iOrderServer.CreateOrders(orders);
            if (resultJson.StatusCode == 400)
            {
                ILoggingService.Error(this._iLogger, "Tạo đơn hàng lỗi , message: " + resultJson.Message,
                    "UserId: " + UserInfo.UserId);

                return Json(new
                {
                    code = 400,
                    content = resultJson.Message,
                    msg = "fail"
                });
            }

            ToastMessage(1, "Tạo đơn hàng thành công !");
            ILoggingService.Infor(this._iLogger, "Tạo đơn hàng thành công , Id: " + orders.Id,
                "UserId: " + UserInfo.UserId);

            return Json(new
            {
                code = 200,
                content = resultJson.Data,
                msg = "Tạo order thành công"
            });
        }
        catch (Exception ex)
        {
            ILoggingService.Error(this._iLogger, "Tạo đơn hàng thành công  ", "UserId: " + UserInfo.UserId, ex);

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
                pttt = "Thanh toán khi nhận hàng";
            }
            else if (order.PaymentType == 1)
            {
                pttt = "Thẻ tín dụng";
            }
            else if (order.PaymentType == 2)
            {
                pttt = "Thẻ ATM nội địa (Internet  banking) ";
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
            ToastMessage(-1, "Dữ liệu code không tồn tại");
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
                        var rs = this._iOrderService.UpdateOrderStatus(order, OrderStatusConst.StatusOrderConfirm,
                            UserInfo);
                        if (rs)
                        {
                            if (order.CustomerId.HasValue)
                            {
                                this._iCustomerNotificationService.SendCustomerNotification(order.CustomerId.Value,
                                    new CustomerNotificationObject()
                                    {
                                        Title = $"PRUgift đã xác nhận đơn hàng {order.Code}",
                                        Detail = "",
                                        Link = $"/account/purchase/{order.Code}"
                                    });
                            }

                            ToastMessage(1, "Cập nhật trạng thái đơn hàng thành công");
                            return Ok(new OutputObject(200, "", $"Cập nhật trạng thái đơn hàng thành công").Show());
                        }
                        else
                        {
                            return Ok(new OutputObject(404, "", $"Cập nhật trạng thái đơn hàng lỗi").Show());
                        }
                    }
                    else
                    {
                        return Ok(new OutputObject(404, "",
                                $"Trạng thái đơn hàng đang là {OrderStatusConst.ListStatus.FirstOrDefault(x => x.Key == order.Status)!.Value}, bạn không thể cập nhật")
                            .Show());
                    }
                }
            }

            return Ok(new OutputObject(404, "", "Đơn hàng không tồn tại").Show());
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"Mã order {data}");
            return Ok(new OutputObject(500, "", "Đơn hàng không tồn tại").Show());
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
                                        $"{(!string.IsNullOrEmpty(ship?.Err) ? ship?.Err : "Cập nhật trạng thái đơn hàng gửi vận chuyển lỗi")}")
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
                                ToastMessage(1, "Cập nhật trạng thái đơn hàng thành công");
                                return Ok(new OutputObject(200, "", $"Gửi đơn hàng thành công").Show());
                            }
                            else
                            {
                                return Ok(new OutputObject(404, "", $"Cập nhật trạng thái đơn hàng gửi vận chuyển lỗi")
                                    .Show());
                            }
                        }
                        else
                        {
                            return Ok(
                                new OutputObject(404, "", $"Cập nhật trạng thái đơn hàng gửi vận chuyển lỗi").Show());
                        }
                    }
                    else
                    {
                        return Ok(new OutputObject(404, "",
                                $"Trạng thái đơn hàng đang là {OrderStatusConst.ListStatus.FirstOrDefault(x => x.Key == order.Status)!.Value}, bạn không thể cập nhật")
                            .Show());
                    }
                }
            }

            return Ok(new OutputObject(404, "", "Đơn hàng không tồn tại").Show());
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"Mã order {data}");
            return Ok(new OutputObject(500, "", "Đơn hàng không tồn tại").Show());
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
                                    Title = $"Đơn hàng {order.Code} đã được giao thành công",
                                    Link = $"/account/purchase/{order.Code}",
                                    Detail = "Cảm ơn Quý Khách hàng đã đồng hành cùng PruGift"
                                });
                        }

                        ToastMessage(1, "Cập nhật trạng thái đơn hàng thành công");
                        return Ok(new OutputObject(200, "", $"Cập nhật trạng thái đơn hàng thành công").Show());
                    }
                    else
                    {
                        return Ok(new OutputObject(404, "", $"Cập nhật trạng thái đơn hàng lỗi").Show());
                    }
                }
            }

            return Ok(new OutputObject(404, "", "Đơn hàng không tồn tại").Show());
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, $"Mã order {data}");
            return Ok(new OutputObject(500, "", "Đơn hàng không tồn tại").Show());
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
                var rs = this._iOrderService.CancelOrders(data.Id, data.Note);
                if (rs.StatusCode == 200)
                {
                    ToastMessage(1, $"Hủy đơn hàng {data.Id} thành công");
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
            this._iLogger.LogError(ex, $"Mã order {data}");
            return Ok(new OutputObject(500, "", "Đơn hàng không tồn tại").Show());
        }
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
                    return Ok(new OutputObject(500, "", "Đơn hàng không tồn tại").Show());
                }

                o.StatusPayment = data.Status;
                this._iOrdersRepository.Update(o);
                ToastMessage(1, $"Cập nhật trạng thái thanh toán đơn hàng {data.OrderCode} thành công");
                return Ok(new OutputObject(200, "",
                    $"Cập nhật trạng thái thanh toa đơn hàng {data.OrderCode} thành công").Show());
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
            this._iLogger.LogError(ex, $"{UserInfo.UserId} câp nhật trạng thái thanh toán mã order {data.OrderCode}");
            return Ok(new OutputObject(500, "", "Đơn hàng không tồn tại").Show());
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
                msg = "Lấy GetOrderEdit thành công"
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
            msg = "Lấy GetOrderEdit lỗi"
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
                msg = "Lấy d/s sản phẩm thành công"
            });
        }
        catch (Exception ex)
        {
            this._iLogger.LogError($"Lấy d/s sản phẩm lỗi , UserId: {UserInfo.UserId} . {ex.Message}");
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
                    msg = "Lấy GetCustomer lỗi"
                });
            }

            rs.FullName = $"{rs.UserName} - {rs.FullName}";
            return Json(new
            {
                code = 200,
                content = rs,
                msg = "Lấy GetCustomer thành công"
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
            msg = "Lấy GetCustomer lỗi"
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
                msg = "Lấy GetPriceProduct thành công"
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
            msg = "Lấy GetCustomer lỗi"
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
                    msg = "Không có sản phẩm trong đơn hàng",
                    content = ""
                });
            }

            // tạo địa chỉ
            var newAddress = new OrderAddress();

            newAddress.Address = model.Address;
            newAddress.Name = model.Name;
            newAddress.Email = model.Email;
            newAddress.Note = model.Note;
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
            order.Point = model.Point;
            order.PrCode = model.PrCode;
            order.PointDiscount = model.Point * IsPoi;
            order.PrFile = !string.IsNullOrEmpty(prFileUrl) ? prFileUrl : order.PrFile;
            order.BillCompanyName =
                customer.Type == 2 ? PrudentialBillInfo.BillCompanyName : model.BillCompanyName;
            order.BillTaxCode = customer.Type == 2 ? PrudentialBillInfo.BillTaxCode : model.BillTaxCode;
            order.BillAddress = customer.Type == 2 ? PrudentialBillInfo.BillAddress : model.BillAddress;
            order.BillEmail = customer.Type == 2 ? PrudentialBillInfo.BillEmail : model.BillEmail;
            modelApi.Order = order;
            modelApi.OrderAddress = newAddress;
            modelApi.IsChangeProduct = isChangProduct;
            modelApi.IsChangePoi = model.CheckChangePoi;
            ResultJson resultJson = _iOrderServer.EditOrders(modelApi);
            if (resultJson.StatusCode == 400)
            {
                ILoggingService.Error(this._iLogger, "Sửa đơn hàng lỗi  " + resultJson.Message,
                    "UserId: " + UserInfo.UserId);

                return Json(new
                {
                    code = 400,
                    content = resultJson.Message,
                    msg = "fail"
                });
            }

            ToastMessage(1, "Sửa đơn hàng thành công !");
            ILoggingService.Infor(this._iLogger, "Sửa đơn hàng thành công Id: " + order.Id,
                "UserId: " + UserInfo.UserId);

            return Json(new
            {
                code = 200,
                content = resultJson.Data,
                msg = "sửa order thành công"
            });
        }
        catch (Exception ex)
        {
            ILoggingService.Error(this._iLogger, "Sửa đơn hàng lỗi  ", "UserId: " + UserInfo.UserId, ex);

            this._iLogger.LogError($"Sửa order lỗi , UserId: {UserInfo.UserId} . {ex.Message}");
            return Json(new
            {
                code = 400,
                msg = "fail",
                content = ex.Message
            });
        }
    }
}