using System.Collections.Generic;
using System.Linq;

namespace CMS.Areas.Categories.Const;

public class BannerConst
{
    public static Dictionary<int, string> ListStatus = new Dictionary<int, string>()
    {
        {1 , "slide"},
        {2 , "popupHome"},
        {3 , "adsArticle"},
        {4 , "adsHomeLeft"},
        {5 , "adsArticle"},
    };

    public static string GetNameListStatus(int key)
    {
        var value = ListStatus.Where(x => x.Key == key).Select(x => x.Value).FirstOrDefault();
        return value;
    } 
}