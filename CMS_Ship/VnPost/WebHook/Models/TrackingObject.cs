namespace CMS_Ship.VnPost.WebHook.Models;

public class TrackingObject
{
    public string OrderCode { get; set; }
    
    public string OrderStatusId { get; set; } 
    
    public string ItemCode { get; set; }
    public string ServiceName { get; set; }
    
    public string TotalFreightIncludeVatEvaluation { get; set; }
}