namespace CMS_Ship.VnPost.Models;

public class CreateOrderOutPut
{
    public string? OrderCode { get; set; }
    public string? ShipCodeIdVnPost { get; set; }
    public string? ExpectedDeliveryTime { get; set; }
    
    public int? TotalFee { get; set; }
    public string? Err { get; set; }
}