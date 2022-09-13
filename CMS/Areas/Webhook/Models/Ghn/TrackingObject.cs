namespace CMS.Areas.Webhook.Models.Ghn;

public class TrackingObject
{
    public string ClientOrderCode { get; set; }
    
    public string OrderCode { get; set; }
    
    public string Status { get; set; }
    
    public string Type { get; set; }
    
    public string Description { get; set; }
}