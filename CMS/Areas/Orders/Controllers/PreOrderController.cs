using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CMS_Access.Repositories.Customers;
using CMS_Access.Repositories.PreOrders;
using CMS_EF.Models.Customers;
using CMS_EF.Models.Orders;
using CMS.Areas.Orders.Const;
using CMS.Areas.Orders.Models.PreOrder;
using CMS.Areas.Orders.Servers;
using CMS.Controllers;
using CMS.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Orders.Controllers;


[Area("Orders")]
public class PreOrderController: BaseController
{
    private readonly ILogger<PreOrderController> _iLogger;
    private readonly IPreOrderRepository _preOrderRepository;
    private readonly IOrderServer _orderServer;
    private readonly ICustomerCouponRepository _customerCouponRepository;
    private readonly ICustomerPointRepository _customerPointRepository;
    public PreOrderController(IPreOrderRepository preOrderRepository, ICustomerCouponRepository customerCouponRepository, ICustomerPointRepository customerPointRepository, IOrderServer orderServer,
        ILogger<PreOrderController> iLogger)
    {
        _preOrderRepository = preOrderRepository;
        _customerCouponRepository = customerCouponRepository;
        _customerPointRepository = customerPointRepository;
        _orderServer = orderServer;
        _iLogger = iLogger;
    }

    // GET
    [Authorize(Policy = "PermissionMVC")]
    [Obsolete]
    public IActionResult Index(int? status,string startDate, string endDate, int pageindex = 1)
    {
        try
        {
            IndexPreOrderViewModel rs = new IndexPreOrderViewModel();
        
            var query = _preOrderRepository.FindAllInfo();
            
            if (status != -1 && status != null)
            {
                query = query.Where(x => x.Status == status);
            }
            
            if (!string.IsNullOrEmpty(startDate))
            {
                var start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                query = query.Where(x => x.PreOrderAt >= start);
            }
            
            if (!string.IsNullOrEmpty(endDate))
            {
                var end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                query = query.Where(x => x.PreOrderAt <= end);
            }
            
            var listData = PagingList.Create(query.OrderByDescending(x => x.PreOrderAt), PageSize, pageindex);
        
            listData.RouteValue = new RouteValueDictionary
            {
                {"status", status},
                {"startDate", startDate},
                {"endDate", endDate}
            };

            List<PreOrderStatus> preOrderStatuses = PreOrderStatus.PreOrderStatusList;

            rs.ListData = listData;
            rs.PreOrderStatuses = preOrderStatuses;
        
            return View(rs);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Details(int id)
    {
        var preOrder = _preOrderRepository.FindAllInfo(id);
   
        if (preOrder == null)
        {
            return NotFound();
        }
        DetailPreOrderViewModel model = new DetailPreOrderViewModel();
        model.PreOrder = preOrder;
        
        List<CustomerCoupon> listCoupon =  _customerCouponRepository.GetALlByCustomerId( preOrder.CustomerId );
        model.Coupons = listCoupon;
        
        List<CustomerPoint> customerPoints = _customerPointRepository.FindByCustomerId(preOrder.CustomerId);
        model.Point = customerPoints.Sum( x => x.Point ) ?? 0;
        
        return View(model);
    }

    [HttpPost]
    public IActionResult CreateOrder([FromForm] CreateOrderViewModel model)
    {
        if (ModelState.IsValid)
        {
            _iLogger.LogInformation("CreateOrder tu pre order id="+model.PreOrderId);
            // Lấy Pre Order
            var preOrder = _preOrderRepository.FindAllInfo(model.PreOrderId);
   
            if (preOrder == null)
            {
                return NotFound();
            }

            //check còn hàng
            if (preOrder.Quantity > (preOrder.ProductSimilar?.QuantityWh ?? 0))
            {
                ToastMessage(-1, "Số lượng sản phẩm trong kho không đủ, bạn vui lòng kiểm tra và thử lại");
                return RedirectToAction("Details", new { id = model.PreOrderId });
            }
            
            // lấy Coupon
            var coupon =  _customerCouponRepository.FindByCustomerId(model.Coupon,preOrder.CustomerId);
            
            // láy product
            List<OrderProduct> orderProducts = new List<OrderProduct>();
            if (preOrder.Product != null && preOrder.ProductSimilar != null)
            {
                List<OrderProductSimilarProperty> properties = preOrder
                    .ProductSimilar
                    ?.ProductSimilarProperty
                    ?.Select(s => new OrderProductSimilarProperty
                    {
                        Flag = 0,
                        LastModifiedAt = DateTime.Now,
                        ProductPropertiesId = s?.ProductPropertiesValue?.ProductProperties?.Id,
                        ProductPropertiesName = s?.ProductPropertiesValue?.ProductProperties?.Name,
                        ProductPropertiesValueId = s?.ProductPropertiesValue?.Id,
                        ProductPropertiesValueName = s?.ProductPropertiesValue?.Value,
                        ProductPropertiesValueNonName = s?.ProductPropertiesValue?.NonValue
                    }).ToList();
                
                OrderProduct product = new OrderProduct()
                {
                    Flag = 0,
                    Price = preOrder.ProductSimilar!.Price,
                    Quantity = preOrder.Quantity,
                    Weight = preOrder.Quantity * preOrder.Product.Weight,
                    PriceSale = preOrder.Product.PriceSale,
                    ProductId = preOrder.ProductId,
                    ProductImage = preOrder.Product.Image,
                    ProductName = preOrder.Product.Name,
                    LastModifiedAt = DateTime.Now,
                    ProductSimilarId = preOrder.ProductSimilarId,
                    ProductSimilarCodeWh = preOrder.ProductSimilar.Skuwh,
                    OrderProductSimilarProperty = properties
                };
                orderProducts.Add(product);
            }

            // tạo địa chỉ
            OrderAddress orderAddress = new OrderAddress
            {
                Address = preOrder.PreOrderAddress.Address,
                Flag = 0,
                Email =  preOrder.PreOrderAddress.Email,
                Name =  preOrder.PreOrderAddress.Name,
                Note = preOrder.PreOrderAddress.Note,
                Phone = preOrder.PreOrderAddress.Phone,
                DistrictCode = preOrder.PreOrderAddress.DistrictCode,
                CommuneCode = preOrder.PreOrderAddress.CommuneCode,
                ProvinceCode = preOrder.PreOrderAddress.ProvinceCode,
                LastModifiedAt = DateTime.Now
            };

            //tao order
            double? price = orderProducts.Sum(x => x.Quantity * x.Price);
            double? totalWeight = orderProducts.Sum(x => x.Quantity * x.Weight);
            var customerType = CustomerTypeConst.PrudentialCustomer.Type;
            var orders = new CMS_EF.Models.Orders.Orders
            {
                Status = OrderStatusConst.StatusWaitCustomerConfirm,
                CustomerId = preOrder!.CustomerId,
                PriceShip = preOrder.PriceShip,
                PriceShipNonSale = preOrder.PriceShipNonSale,
                PriceShipSalePercent = preOrder.PriceShipSalePercent,
                PaymentType = preOrder.PaymentType,
                ShipPartner = preOrder.ShipPartner,
                ShipType = preOrder.ShipType,
                OrderAt = DateTime.Now,
                LastModifiedAt = DateTime.Now,
                CouponCode = coupon?.Code ?? "",
                CouponDiscount = coupon?.ReducedPrice ?? 0,
                PointDiscount = model.Point * PointConst.Coefficient,
                Point = model.Point,
                Note = "",
                BillCompanyName =  preOrder.Customer.Type == customerType ? PrudentialBillInfo.BillCompanyName : preOrder.BillCompanyName,
                BillTaxCode = preOrder.Customer.Type == customerType ? PrudentialBillInfo.BillTaxCode : preOrder.BillTaxCode,
                BillAddress = preOrder.Customer.Type == customerType ? PrudentialBillInfo.BillAddress : preOrder.BillAddress,
                BillEmail = preOrder.Customer.Type == customerType ? PrudentialBillInfo.BillEmail : preOrder.BillEmail,
                PrFile = preOrder.PrFile,
                PrCode = preOrder.PrCode,
                Price = price,
                TotalWeight = totalWeight,
                Total = price + preOrder.PriceShip - (coupon?.ReducedPrice ?? 0) - (model.Point ?? 0) * PointConst.Coefficient,
                OrderProduct = orderProducts,
                OrderAddress = orderAddress
            };
            
            
            ResultJson resultJson = _orderServer.CreateOrders(orders);
            if (resultJson.StatusCode == 200)
            {
                preOrder.Status = PreOrderStatus.Confirmed.Status;
                preOrder.OrderCode = resultJson.Data;
                _preOrderRepository.Update(preOrder);  
                _iLogger.LogInformation("Tạo đơn hàng thành công");
                ToastMessage(1, "Tạo đơn hàng thành công");
            }
            else
            {
                
                _iLogger.LogError(resultJson.Message + model.PreOrderId);
                ToastMessage(-1, resultJson.Message);
            }
            
        }
        else
        {
            ToastMessage(-1, "Tạo đơn hàng thất bại");
        }

        return RedirectToAction("Details", new { id = model.PreOrderId });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public JsonResult Delete(int? id)
    {
        try
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu");
                _iLogger.LogError("Không có dữ liệu");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu trong hệ thống, không thể xóa"
                });
            }
            var order = _preOrderRepository.FindById((int) id);
            if (order == null || order.Status != PreOrderStatus.NotProcessedYet.Status)
            {
                ToastMessage(-1, "Không có dữ liệu");
                _iLogger.LogError("Không có dữ liệu");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu trong hệ thống, không thể xóa"
                });
            }
            
            _preOrderRepository.Delete(order);
            
            ToastMessage(1, "Xóa dữ liệu thành công");
            _iLogger.LogInformation("Xóa dữ liệu thành công");
            return Json(new
            {
                msg = "successful",
                content = "Xóa dữ liệu thành công"
            });
        }
        catch (Exception)
        {
            ToastMessage(-1, "Xóa dữ liệu lỗi");
            _iLogger.LogError("Xóa dữ liệu lỗi");
            return Json(new
            {
                msg = "fail",
                content = "Không thể xóa dữ liệu, liên hệ người quản trị"
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public JsonResult Cancel(int id)
    {
        try
        {
            
            var order = _preOrderRepository.FindById(id);
            if (order == null || order.Status != PreOrderStatus.NotProcessedYet.Status)
            {
                ToastMessage(-1, "Không có dữ liệu");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu trong hệ thống, không thể hủy"
                });
            }

            order.Status = PreOrderStatus.Cancel.Status;
            
            _preOrderRepository.Update(order);
            _iLogger.LogInformation("Hủy Pre Order thành công");
            ToastMessage(1, "Hủy Pre Order thành công");
            return Json(new
            {
                msg = "successful",
                content = "Hủy Pre Order thành công"
            });
        }
        catch (Exception)
        {
            _iLogger.LogError("Hủy Pre Order thất bại");
            ToastMessage(-1, "Hủy Pre Order thất bại");
            return Json(new
            {
                msg = "fail",
                content = "Hủy Pre Order thất bại"
            });
        }
    }
}