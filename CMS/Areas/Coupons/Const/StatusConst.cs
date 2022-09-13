namespace CMS.Areas.Coupons.Const;

public class StatusConst
{
    public static string BindStatus(bool status)
    {
        if (status )
        {
            return
                $"<span class=\"status badge bg-info text-dark p-2\">Đã sử dụng</span>";
        }
        else
        {
            return
                $"<span class=\"status badge bg-secondary text-dark\">Chưa sử dụng</span>";
        }
    }
}