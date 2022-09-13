using CMS_Lib.Extensions.Attribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Webhook.Controllers;

[Area("Webhook")]
[NonLoad]
public class KiotVietController : Controller
{
    private readonly ILogger<KiotVietController> _iLogger;
    private readonly string webHookToken;
    public KiotVietController(ILogger<KiotVietController> iLogger, IConfiguration iConfiguration)
    {
        this._iLogger = iLogger;
        this.webHookToken = iConfiguration.GetSection("AppSetting:WebhookToken").Value;
    }
    
    // Webhook/KiotViet/ReceiveWebhook?name=zsfhdalb
    [HttpPost]
    public IActionResult ReceiveWebhook([FromQuery] string name,[FromBody] object req)
    {
        if (name == webHookToken)
        {
            this._iLogger.LogInformation($"webhook check kiot Viet: {req}");
        }
        else
        {
            this._iLogger.LogInformation($"webhook kiot Viet: {req}");
        }
        return Ok("ok");
    }
}