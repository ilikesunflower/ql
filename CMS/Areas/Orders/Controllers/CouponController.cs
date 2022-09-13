using System;
using System.Collections.Generic;
using System.Linq;
using CMS.Controllers;
using CMS_Access.Repositories.Customers;
using CMS_Access.Repositories.Orders;
using CMS_EF.Models.Customers;
using CMS_Lib.Extensions.Attribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Orders.Controllers;

[Area("Orders")]
[NonLoad]
public class CouponController : BaseController
{
    private readonly ILogger _iLogger;
    private readonly ICustomerCouponRepository _iCustomerCouponRepository;
    private readonly IOrdersRepository _iOrdersRepository;
    public CouponController(ILogger<CouponController> iLogger, ICustomerCouponRepository iCustomerCouponRepository, IOrdersRepository iOrdersRepository)
    {
        _iLogger = iLogger;
        _iCustomerCouponRepository = iCustomerCouponRepository;
        _iOrdersRepository = iOrdersRepository;
    }
    
    [HttpGet]
    public IActionResult CouponGetCustomer(int? customerId)
    {
        try
        {
            List<CustomerCoupon> coupons =  _iCustomerCouponRepository.GetALlByCustomerId(customerId);
           
            return Json(new
            {
                code = 200,
                content = coupons,
                msg ="Lấy d/s coupon thành công"
            });
        }
        catch (Exception e)
        {
            _iLogger.LogError(e.Message);
            return Json(new
            {   code = 400,
                msg = "not found",
                content = ""
            });
        }
    } 
    
    [HttpGet]
    public IActionResult CouponGetCustomerEdit(int? customerId , int? orderId)
    {
        try
        {
            var order = _iOrdersRepository.FindById(orderId ?? 0);
            List<CustomerCoupon> coupons =  _iCustomerCouponRepository.GetALlByCustomerIdOrderId(customerId, order?.CouponCode);
           
            return Json(new
            {
                code = 200,
                content = coupons,
                msg ="Lấy d/s coupon thành công"
            });
        }
        catch (Exception e)
        {
            _iLogger.LogError(e.Message);
            return Json(new
            {   code = 400,
                msg = "not found",
                content = ""
            });
        }
    }

}