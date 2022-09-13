using System.Collections.Generic;

namespace CMS_App_Api.Areas.Orders.Const;

public class OrderStatusConst
{
    public static OrderStatusConst CustomerConfirm => new(0, "Xác nhận đặt hàng");
    public static OrderStatusConst PruConfirm => new(1, "Chờ xác nhận");
    public static OrderStatusConst Confirmed => new(2, "Chờ lấy hàng");
    public static OrderStatusConst Shipping => new(3, "Đang giao");
    public static OrderStatusConst Success => new(4, "Đã giao");
    public static OrderStatusConst Failed => new(5, "Đã hủy");

    public static List<int?> FollowListOrder = new()
        { CustomerConfirm.Status, PruConfirm.Status, Confirmed.Status, Shipping.Status };

    
    public OrderStatusConst(int status, string name)
    {
        Status = status;
        Name = name;
    }


    public int Status { set; get; }
    public string Name { set; get; }
}