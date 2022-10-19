namespace CMS.Areas.Categories.Const;

public static class StatusConst
{
    public const int ArticleShow = 1;

    public static string BindStatus(int status)
    {
        return status == 1
            ? "<span class=\"status badge bg-info text-dark p-2\">Kích hoạt</span>"
            : "<span class=\"status badge bg-secondary text-dark\">Không kích hoạt</span>";
    }

    public static string BindStatus(bool? status)
    {
        return status == true
            ? "<span class=\"status badge bg-success text-dark p-2\"> Đã duyệt </span>"
            : "<span class=\"status badge bg-secondary text-dark p-2\"> Chờ duyệt </span>";
    }
    public static string BindStatusText(bool? status)
    {
        return status == true
            ? " Đã duyệt "
            : " Chờ duyệt ";
    }
}