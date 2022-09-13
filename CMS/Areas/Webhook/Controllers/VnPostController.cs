using System;
using System.Security.Cryptography;
using System.Text;
using CMS.Areas.Customer.Services;
using CMS.Areas.Webhook.Models.VnPost;
using CMS_Lib.Extensions.Attribute;
using CMS_Ship.VnPost.WebHook;
using CMS_Ship.VnPost.WebHook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CMS.Areas.Webhook.Controllers;

[Area("Webhook")]
[NonLoad]
public class VnPostController : Controller
{
    private readonly ILogger<VnPostController> _iLogger;
    private readonly string _xmlPublicKey;
    private readonly string _webHookToken;
    private readonly ITrackingService _iTrackingService;
    private readonly ICustomerNotificationService _iCustomerNotificationService;

    
    public VnPostController(ILogger<VnPostController> iLogger, IConfiguration iConfiguration, ITrackingService iTrackingService, ICustomerNotificationService iCustomerNotificationService)
    {
        this._iLogger = iLogger;
        _iTrackingService = iTrackingService;
        _iCustomerNotificationService = iCustomerNotificationService;
        _xmlPublicKey =
            "<RSAKeyValue><Modulus>3G3hLSrPwF4FBBA/0yEZkNwX2++SSCIaGKeb8TBb6loc3NRSvo0oDR0dO7c6bk/CgizQ7ZT0d/rlZunV4UbP2gzVl3p6VN2ykoDhnmdGClk1+js6EqWIWztZrcF2mAq0s3OHIH4tLnLIGWbMws1nQNRUoJDwfGVSZcLzFnWRWb21kYjpSbu44Y2IiQX6y3n2YR9VPyxI9VMYkrTvdzN/cTFyRhrPaH15pXzkQ8zQl561mSYGcucJl56GX9hRaho5zuNSNWq+oVXdIBE6UOVVX4TXJJJw+iKlLYO/2OryJ3fNLKWajBaYGzxZ6QLjpfr/HYtAPGLARLtDtean7JjE5Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        this._webHookToken = iConfiguration.GetSection("AppSetting:WebhookToken").Value;
    }
    
    // Webhook/VnPost/ReceiveWebhook?name=zsfhdalb
    [HttpPost]
    public IActionResult ReceiveWebhook([FromQuery] string name, [FromBody] OrderMessage req)
    {
        try
        {
            if (name != _webHookToken)
            {
                return Ok("ok");
            }
            string json = ToJson(req);
            byte[] b = Encoding.UTF8.GetBytes(req.Data + req.SendDate);
            bool isSuccess = VerifySignature(b, Convert.FromBase64String(req.SignData!), _xmlPublicKey);
            if (isSuccess)
            {
                var data = JObject.Parse(req.Data);
                TrackingObject trackingObject = new TrackingObject()
                {
                    ItemCode = $"{data["ItemCode"]}",
                    OrderCode = $"{data["OrderCode"]}",
                    ServiceName = $"{data["ServiceName"]}",
                    OrderStatusId = $"{data["OrderStatusId"]}",
                    TotalFreightIncludeVatEvaluation = $"{data["TotalFreightIncludeVatEvaluation"]}",
                };
                var order = this._iTrackingService.InsertTracking(trackingObject);
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
            this._iLogger.LogInformation(ex,$"ReceiveWebhook: req {req}");
        }
        return Ok("ok");
    }

    private bool VerifySignature(byte[] data, byte[] signature, string publicKey)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(publicKey);
        // the hash to sign
        byte[] hash;
        using (var sha256 = SHA256.Create())
        {
            hash = sha256.ComputeHash(data);
        }

        var rsaFormatter = new RSAPKCS1SignatureDeformatter(rsa);
        rsaFormatter.SetHashAlgorithm("SHA256");
        return rsaFormatter.VerifySignature(hash, signature);
    }

    private string? ToJson(object? input)
    {
        return input == null ? null : JsonConvert.SerializeObject(input);
    }
    
}