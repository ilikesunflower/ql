using System.Collections.Generic;

namespace CMS.Areas.Admin.Const
{
    public class UserConst
    {
        public static Dictionary<int, string> ListTypeUser = new Dictionary<int, string>()
        {
            { 0, "Tài khoản thường" },
            { 1, "Tài khoản SSO" },
        };

        public static string GetTypeUser(int type)
        {
            if (type == 0)
            {
                return "<span class='badge badge-primary'>Tài khoản thường</span>";
            }else if (type == 1)
            {
                return "<span class='badge badge-warning'>Tài khoản SSO</span>";
            }
            return "";
        }
    }
}
