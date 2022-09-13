using System;
using System.Collections.Generic;
using System.Linq;
using CMS.Areas.Orders.Servers;
using CMS.Controllers;
using CMS_Access.Repositories.Customers;
using CMS_EF.Models.Customers;
using CMS_EF.Models.Orders;
using CMS_Lib.Extensions.Attribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Orders.Controllers;
[Area("Orders")]
[NonLoad]
public class PointController : BaseController
{
    // GET
    private readonly ILogger _iLogger;
    private readonly ICustomerPointRepository _iCustomerPointRepository;
    public PointController(ILogger<PointController> iLogger, ICustomerPointRepository iCustomerPointRepository)
    {
        _iLogger = iLogger;
        _iCustomerPointRepository = iCustomerPointRepository;
    }
    
    [HttpGet]
    public IActionResult PointOfCustomer(int customerId)
    {
        try
        {
            List<CustomerPoint> point = _iCustomerPointRepository.FindByCustomerId(customerId);
            int quantityPoi = (int)point.Sum(x => x.Point);
            return Json(new
            {
                code = 200,
                content = quantityPoi,
                msg ="Lấy d/s sản phẩm thành công"
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

    public IActionResult PointOldCustomer( int orderId)
    {
        try
        {
            List<OrderPoint> point = _iCustomerPointRepository.FindByIdAndOrderId(orderId);
            int quantityPoi = (int)point.Sum(x => x.Point);
            return Json(new
            {
                code = 200,
                content = quantityPoi,
                msg ="Lấy số lượng point thành công"
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
