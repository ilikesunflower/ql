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
                return Ok(new OutputObject(MessageCode.Success, order.Code , "Successful").Show());
            }
            catch (Exception ex)
            {
                _iLogger.LogDebug(ex,"Create Error:"+ex.Message);
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
                return Ok(new OutputObject(MessageCode.Success, new {} , "Successful").Show());
            }
            catch (Exception ex)
            {
                _iLogger.LogDebug(ex,"Cancel Error:"+ex.Message);
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
                return Ok(new OutputObject(MessageCode.Success, order.Code , "Successful").Show());
            }
            catch (Exception ex)
            {
                return Ok(new OutputObject(MessageCode.BadRequest, new{}, ex.Message,ex.Message).Show());
            }
        }
    }
}
