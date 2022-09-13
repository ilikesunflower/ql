using System.Collections.Generic;
using System.Linq;

namespace CMS.Areas.Products.Const;

public class ProductSexConst
{
    public static Dictionary<int, string> ListStatus = new Dictionary<int, string>()
    {
        {0 , "Tất cả "},
        {1 , "Nam"},
        {2 , "Nữ"},
    };

    public static string GetProductSex(int key)
    {
        return ListStatus.Where(x => x.Key == key).Select(x => x.Value).FirstOrDefault();
    }
}