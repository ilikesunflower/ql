using System.Collections.Generic;
using System.Linq;

namespace CMS.Areas.Customer.Const;

public class CustomerTypeGroupConst
{
    public static int Staff = 1;
    public static string StaffText = "Staff";
    public static int PhongBan = 2;
    public static int GA = 3;
    public static string GAText = "GA";

    public static Dictionary<int, string> ListCustomerTypeGroupConst = new()
    {
        { Staff, "Staff" },
        { PhongBan, "Phòng ban" },
        { GA, "GA" },
    };

    public static Dictionary<int, string> ListCustomerTypeGroupConstColor = new Dictionary<int, string>()
    {
        { Staff, "<span class='badge badge-success'>Staff</span>" },
        { PhongBan, "<span class='badge badge-primary'>Phòng ban</span>" },
        { GA, "<span class='badge badge-info'>GA</span>" },
    };

    public static string GetCustomerTypeGroup(int type)
    {
        return ListCustomerTypeGroupConst.Where(x => x.Key == type).Select(x => x.Value).FirstOrDefault();
    }
}