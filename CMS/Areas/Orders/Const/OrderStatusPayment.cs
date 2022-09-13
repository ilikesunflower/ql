using System.Collections.Generic;
using System.Linq;

namespace CMS.Areas.Orders.Const;

public class OrderStatusPayment
{
    public static int StatusNoPayment = 0;
    public static int StatusSuccess = 1;

    public static Dictionary<int, string> ListOrderStatusPayment = new()
    {
        {StatusNoPayment, "Chưa thanh toán"},
        {StatusSuccess, "Đã thanh toán"}
    };

    public static string BindStatus(int? status)
    {
        if (!status.HasValue)
        {
            return "<span class='status badge bg-secondary text-white'>Chưa thanh toán</span>";
        }
        KeyValuePair<int,string>? d = ListOrderStatusPayment.FirstOrDefault(x => x.Key == status);
        if (d.Value.Key == StatusNoPayment)
        {
            return $"<span class='status badge bg-secondary text-white'>{d.Value.Value}</span>";
        }
        if (d.Value.Key == StatusSuccess)
        {
            return $"<span class='status badge bg-success text-white'>{d.Value.Value}</span>";
        }
        return "";
    }
    public static string BindStatusStr(int? status)
    {
        if (!status.HasValue)
        {
            return "Chưa thanh toán";
        }
        KeyValuePair<int,string>? d = ListOrderStatusPayment.FirstOrDefault(x => x.Key == status);
        return d.Value.Value;
    }
}