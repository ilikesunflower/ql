namespace CMS_Ship.GHN.Models;

public class CreatedOrder
{
    public string OrderCode { get; set; }
    
    public string? ToName { get; set; }
    public string? ToPhone { get; set; }
    public string? ToAddress { get; set; }
    public string? ToWardCode { get; set; }
    public string? ToDistrictId { get; set; }
    
    
    public int? CodAmount { get; set; }
    public string? Content { get; set; }
    
    public int? Weight { get; set; }
    
    public int? ServiceTypeId { get; set; }
    
    public int? PaymentTypeId { get; set; }
    
    public string? Note { get; set; }
    public string? ClientOrderCode { get; set; }
    
    public List<Item> ListItem { get; set; }

}

public class Item
{
    public string name { get; set; }
    public string code { get; set; }
    public int quantity { get; set; }
    public int price { get; set; }
    // public int weight  { get; set; }
}

