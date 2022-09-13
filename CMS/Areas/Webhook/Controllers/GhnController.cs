using System;
using System.Text.Json.Nodes;
using CMS.Areas.Customer.Services;
using CMS.Areas.Webhook.Models.Ghn;
using CMS_EF.Models.Orders;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Json;
using CMS_Ship.GHN.Webhook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CMS.Areas.Webhook.Controllers;

[Area("Webhook")]
[NonLoad]
public class GhnController : Controller
{
    private readonly ILogger<GhnController> _iLogger;
    private readonly string webHookToken;
    private readonly ITrackingService _iTrackingService;
    private readonly ICustomerNotificationService _iCustomerNotificationService;


    public GhnController(ILogger<GhnController> iLogger, IConfiguration iConfiguration, ITrackingService iTrackingService, ICustomerNotificationService iCustomerNotificationService)
    {
        this._iLogger = iLogger;
        _iTrackingService = iTrackingService;
        _iCustomerNotificationService = iCustomerNotificationService;
        this.webHookToken = iConfiguration.GetSection("AppSetting:WebhookToken").Value;
    }

    // Webhook/Ghn/ReceiveWebhook?name=zsfhdalb
    [HttpPost]
    public IActionResult ReceiveWebhook([FromQuery] string name, [FromBody] JObject req)
    {
        try
        {
            // if (name != webHookToken)
            // {
            //     return Ok("ok"); 
            // }
            string clientOrderCode = $"{req["ClientOrderCode"]}";
            string type = $"{req["Type"]}";
            string orderCode = $"{req["OrderCode"]}";
            string status = $"{req["Status"]}";
            string description = $"{req["Description"]}";
            string codTransferDate = $"{req["CODTransferDate"]}";
            CMS_Ship.GHN.Webhook.Models.TrackingObject rs = new CMS_Ship.GHN.Webhook.Models.TrackingObject()
            {
                Description = description,
                Status = status,
                Type = type,
                OrderCode = orderCode,
                ClientOrderCode = clientOrderCode,
                CODTransferDate = codTransferDate
            };
            if (!string.IsNullOrEmpty(clientOrderCode))
            {
               var order = this._iTrackingService.InsertTracking(rs);
               if (order is { CustomerId: { } })
               {
                   _iCustomerNotificationService.SendCustomerNotification(order.CustomerId.Value,new CustomerNotificationObject()
                   {
                       Title = $"Đơn hàng {order.Code} đã được giao thành công",
                       Link = $"/account/purchase/{order.Code}",
                       Detail = "Cảm ơn Quý Khách hàng đã đồng hành cùng PruGift"
                   });
               }
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex,"ghn webhook");
        }
        return Ok("ok");
    } 
}