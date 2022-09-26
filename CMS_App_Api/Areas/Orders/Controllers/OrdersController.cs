using CMS_App_Api.Controllers;
using CMS_App_Api.Helpers.Consts;
using CMS_App_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using CMS_App_Api.Areas.Orders.Models;
using CMS_App_Api.Services.Orders;

namespace CMS_App_Api.Areas.Orders.Controllers
{
    [ApiController]
    public class OrdersController : BaseController
    {
        private readonly ILogger<OrdersController> _iLogger;
        private readonly IOrderServices _orderServices;

        public OrdersController(ILogger<OrdersController> iLogger, IOrderServices orderServices)
        {
            _iLogger = iLogger;
            _orderServices = orderServices;
        }

        [HttpGet("/api/orders")]
        public IActionResult Index()
        {
            return Ok(new OutputObject(MessageCode.Success, new { }, "Successful").Show());
        }
        [HttpPost("/api/orders")]
        public IActionResult Create([FromBody] CMS_EF.Models.Orders.Orders model)
        {
            _iLogger.LogDebug("Create order");
            try
            {
                var order = _orderServices.Create(model);
                this._iLogger.LogInformation($"Tạo đơn hàng thành công cho khách {order.CustomerId}");
                return Ok(new OutputObject(MessageCode.Success, order.Code , "Successful").Show());
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex,$"Tạo đơn hàng cho khách {model.CustomerId} lỗi:");
                return Ok(new OutputObject(MessageCode.BadRequest, new{}, ex.Message,ex.Message).Show());
            }
        }
        
        [HttpPost("/api/orders/{code}")]
        public IActionResult Cancel( string code,[FromBody] CancelOrderModel model)
        {
            _iLogger.LogDebug("Cancel order");
            try
            {
                _orderServices.Cancel(code,model.Message);
                _iLogger.LogInformation($"Hủy đơn hàng {code} thành công");
                return Ok(new OutputObject(MessageCode.Success, new {} , "Successful").Show());
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex,$"hủy đơn hàng {code} lỗi:"+ex.Message);
                return Ok(new OutputObject(MessageCode.BadRequest, new{}, ex.Message,ex.Message).Show());
            }
        }
        
        [HttpPut("/api/orders")]
        public IActionResult Edit([FromBody] OrderEditApiModel model)
        {
            _iLogger.LogDebug("Edit order");
            try
            {
                var order = _orderServices.Edit(model);
                _iLogger.LogInformation($"sửa đơn hàng {order.Code} thành công");
                return Ok(new OutputObject(MessageCode.Success, order.Code , "Successful").Show());
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex,$"sửa đơn hàng {model.Order.Code} lỗi:"+ex.Message);
                return Ok(new OutputObject(MessageCode.BadRequest, new{}, ex.Message,ex.Message).Show());
            }
        }
    }
}
