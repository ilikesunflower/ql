using System.Collections.Generic;
using System.Linq;
using CMS.Areas.Customer.Const;

namespace CMS.Areas.Admin.Const;

public class AreaCost
{
    public static int North = 1;
    public static int Central = 2;
    public static int South = 3;
    public static int WareHouse = 5;
    public static int HCM = 4;

    public static Dictionary<int, string>  ListArea =  new Dictionary<int, string>()
      {
          { North, "Miền Bắc"},
          { Central, "Miền Trung"},
          {South, "Miền Nam"},
          {HCM, "HCM"},
          {WareHouse, "Kho"},
      };

    public static string GetArea(int type)
    {
        return ListArea.Where(x => x.Key == type).Select(x => x.Value).FirstOrDefault();
    }
}
