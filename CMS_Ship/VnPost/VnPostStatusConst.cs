namespace CMS_Ship.VnPost;

public class VnPostStatusConst
{
    public static string ShipSuccess = "100";
    public static string CodSuccess = "120";
    
    public static Dictionary<string, string> ListStatus = new Dictionary<string, string>()
    {
        {"10", "Đã xóa đơn hàng"},
        {"20", "Vào điều tin"},
        {"60", "Hủy đơn hàng"},
        {"61", "Báo hủy"},
        {"62", "Đã nhận báo hủy"},
        {"70", "Bưu cục đã nhận đơn hàng và nhập vào hệ thống chuyển phát"},
        {"91", "Đã đi giao hàng cho người nhận nhưng không thành công"},
        {"100", "Hàng đã giao thành công"},
        {"110", "Bưu tá đã nhận tiền COD của người nhận và nhập vào hệ thống"},
        {"120", "Tiền COD đã được trả cho người gửi"},
        {"161", "Hoàn đơn cho người gửi thất bại"},
        {"170", "Phát lại cho người gửi thành công"},
    };
}