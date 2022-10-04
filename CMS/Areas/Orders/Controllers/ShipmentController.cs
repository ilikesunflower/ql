using System;
using CMS.Areas.Orders.Const;
using CMS.Areas.Orders.Servers;
using CMS.Controllers;
using CMS_Lib.Extensions.Attribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Orders.Controllers;

[Area("Orders")]
[NonLoad]
public class ShipmentController : BaseController
{    
    private readonly IShipmentService _shipmentService;
    private readonly ILogger<ShipmentController> _logger;

    public ShipmentController(IShipmentService shipmentService, ILogger<ShipmentController> logger)
    {
        _shipmentService = shipmentService;
        _logger = logger;
    }
    
    public IActionResult CheckShipmentCost(string provinceCode, string districtCode, string communeCode, int weight)
    {
        try
        {
            if (string.IsNullOrEmpty(provinceCode) || string.IsNullOrEmpty(districtCode) || string.IsNullOrEmpty(communeCode))
            {
                return Json(new
                {   code = 400,
                    msg = "not found",
                    content = ""
                });
            }
            var shipmentCost =  _shipmentService.GetShipmentCost(provinceCode, districtCode, communeCode, weight);
            
                
            return Json(new
            {
                code = 200,
                content = shipmentCost,
                msg ="Lấy d/s shipmentCost thành công"
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e,$"CheckShipmentCost");
            return Json(new
            {   code = 400,
                msg = "not found",
                content = ""
            });
        }
    }
    
    [HttpGet]
    [NonLoad]
    public IActionResult GetListPromotionShip()
    {
        try
        {
            var rs = PromotionShipCost.ListPromotionShipConst;
            return Json(new
            {
                msg = "successful",
                content = rs,
                code = 200
            });
        }
        catch (Exception)
        {
            return Json(new
            {
                msg = "fail",
                content = "Lỗi "
            });
        }
    }

}