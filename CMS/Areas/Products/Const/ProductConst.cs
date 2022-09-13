using System.Collections.Generic;

namespace CMS.Areas.Products.Const;

public class ProductConst
{
    public static bool IsPublish = true;
    public static bool IsNotPublish = false;
    public static Dictionary<int, string> ListStatus = new Dictionary<int, string>()
    {
        {0 , "Tất cả trạng thái"},
        {1 , "Kích hoạt"},
        {2 , "Không kích hoạt"},
    };
    public static Dictionary<int, string> ListStatusTT = new Dictionary<int, string>()
    {
        {0 , "Tất cả loại hàng hóa"},
        {1 , "Nổi bật"},
        {2 , "Mới"},
        {3 , "Bán chạy"},
        {4 , "Khuyến mãi"},
    };
}