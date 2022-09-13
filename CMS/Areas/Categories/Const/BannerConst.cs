using System.Collections.Generic;
using System.Linq;

namespace CMS.Areas.Categories.Const;

public class BannerConst
{
    public static Dictionary<int, string> ListStatus = new Dictionary<int, string>()
    {
        {1 , "slide"},
        {2 , "popupHome"},
    };

    public static string GetNameListStatus(int key)
    {
        var value = ListStatus.Where(x => x.Key == key).Select(x => x.Value).FirstOrDefault();
        return value;
    } 
}