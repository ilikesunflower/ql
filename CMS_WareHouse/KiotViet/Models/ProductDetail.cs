namespace CMS_WareHouse.KiotViet.Models;

public class ProductDetail
{
    public int ProductId { get; set; }
    public int BranchId { get; set; }
    public string Code { get; set; }
    public int Quantity { get; set; }
    public int Cost { get; set; }
}