namespace CMS_Ship.GHN.Models;

public class OrderHook
{
    public int? CODAmount { get; set; }

    public string CODTransferDate { get; set; }

    public string? ClientOrderCode { get; set; }

    public int? ConvertedWeight { get; set; }

    public string? Description { get; set; }

    public string? OrderCode { get; set; }

    public int? PaymentType { get; set; }

    public int? ShopID { get; set; }

    public string? Status { get; set; }

    public string Time { get; set; }

    public int? TotalFee { get; set; }

    public string? Type { get; set; }

    public string? Warehouse { get; set; }
}