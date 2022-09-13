using System.Collections.Generic;
using System.Linq;

namespace CMS.Areas.Orders.Const;

public class OrderStatusConst
{
    public static int StatusWaitCustomerConfirm = 0;
    public static int StatusCustomerSuccess = 1;
    public static int StatusOrderConfirm = 2;
    public static int StatusOrderShip = 3;
    public static int StatusOrderSuccess = 4;
    public static int StatusOrderCancel = 5;
    
    public static Dictionary<int, string> ListStatus = new Dictionary<int, string>()
    {
        {0 , "Chờ khách xác nhận"},
        {1 , "Khách đặt hàng thành công"},
        {2 , "Chờ giao hàng"},
        {3, "Đang vận chuyển"},
        {4, "Giao hàng thành công"},
        {5, "Đã hủy"}
    };
    public static Dictionary<int, string> ListColor = new Dictionary<int, string>()
    {
        {0 , "bg-secondary text-white"},
        {1 , "bg-primary  text-white"},
        {2 , "bg-warning text-dark"},
        {3, "bg-info  text-white"},
        {4, "bg-success  text-white"},
        {5, "bg-danger text-white"}
    };

    public static string GetStatus(int status)
    {
        return   $"<span class=\"status badge {ListColor.Where(x => x.Key == status).Select(x => x.Value).FirstOrDefault() ?? ""}\">{ListStatus.Where(x => x.Key == status).Select(x => x.Value).FirstOrDefault() ?? ""}</span>";
    }
    public static string GetStatusStr(int status)
    {
        return  ListStatus.Where(x => x.Key == status).Select(x => x.Value).FirstOrDefault();
    }
}