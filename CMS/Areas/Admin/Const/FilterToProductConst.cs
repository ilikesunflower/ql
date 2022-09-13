using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace CMS.Areas.Admin.Const;

public class FilterToProductConst
{
    public static int StatusPrice = 0;
    public static int StatusQuantity = 1;
    public static Dictionary<int, string> ListStatus = new Dictionary<int, string>()
    {
        {StatusPrice, "Theo doanh số"},
        {StatusQuantity, "Theo số lượng"},
    };

    public static List<string> GetStringStatus = ListStatus.OrderBy(x => x.Key).Select(x => x.Value).ToList();

    public static string GetValue(int typeStatus)
    {
        return ListStatus.Where(x => x.Key == typeStatus).Select(x => x.Value).FirstOrDefault();
    }
}