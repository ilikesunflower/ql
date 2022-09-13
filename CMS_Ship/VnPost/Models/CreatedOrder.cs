namespace CMS_Ship.VnPost.Models;

public class CreatedOrder
{
    public string OrderCode { get; set; }
    // public string? SenderPhone { get; set; }
    // public string? SenderFullname { get; set; }
    // public string? SenderAddress { get; set; }
    // public string? SenderWardId { get; set; }
    // public string? SenderDistrictId { get; set; }
    // public string? SenderProvinceId { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? ReceiverFullname { get; set; }
    public string? ReceiverAddress { get; set; }
    public string? ReceiverWardId { get; set; }
    public string? ReceiverDistrictId { get; set; }
    public string? ReceiverProvinceId { get; set; }
    
    public int TypeShip { get; set; }
    
    // 1 nhà riêng - 2 cơ quan
    // public int? ReceiverAddressType { get; set; }
    // public string? ServiceName { get; set; }
    // public string? OrderCode { get; set; }
    public string? PackageContent { get; set; }
    public double? WeightEvaluation { get; set; }

    // cho phép xem hàng hay k
    public bool? IsPackageViewable { get; set; }

    // 255 ký tự
    public string? CustomerNote { get; set; }

    // 1 thu gom tận nơi | 2: Gửi hàng tại bưu cục
    // public int? PickupType { get; set; }

    // số tiền thu hộ
    public decimal? CodAmountEvaluation { get; set; }
}