namespace CMS_Ship.GHN;

public class GhnStatusConst
{
    public static string ShipSuccess = "delivered";
    public static Dictionary<string, string> ListStatus = new Dictionary<string, string>()
    {
        { "ready_to_pick", "Đơn hàng vận chuyển vừa được tạo" },
        { "picking", "Shipper đến lấy hàng" },
        { "cancel", "Đơn hàng vận chuyển đã bị hủy" },
        { "money_collect_picking", "Người giao hàng đang tương tác với người bán" },
        { "picked", "Người giao hàng được chọn hàng" },
        { "storing", "Hàng đã được chuyển đến trung tâm phân loại GHN" },
        { "transporting", "Hàng đang được luân chuyển" },
        { "sorting", "Hàng hóa đang được phân loại tại kho" },
        { "delivering", "Shipper đang giao hàng cho khách hàng" },
        { "money_collect_delivering", "Shipper đang tương tác với người mua" },
        { "delivered", "Hàng đã được giao cho khách hàng" },
        { "delivery_fail", "Hàng hóa chưa được giao cho khách hàng" },
        { "waiting_to_return", "Hàng đang chờ giao (có thể giao trong vòng 24 / 48h)" },
        { "return", "Hàng đang chờ trả lại cho người bán (Giao hàng sau 3 lần không thành công)" },
        { "return_transporting", "Hàng đang được luân chuyển" },
        { "return_sorting", "Hàng hóa đang được phân loại tại kho" },
        { "returning", "Shipper đang trả lại cho người bán" },
        { "return_fail", "Trả hàng cho người bán không thành công" },
        { "returned", "Hàng hóa đã được trả lại cho người bán" },
        { "exception", "Xử lý ngoại lệ hàng hóa (các trường hợp làm trái quy trình)" },
        { "damage", "Hàng hóa bị hư hỏng" },
        { "lost", "Hàng bị mất" },
    };
}