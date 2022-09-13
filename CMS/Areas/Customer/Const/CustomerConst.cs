using System.Collections.Generic;

namespace CMS.Areas.Customer.Const;

public class CustomerConst
{
    public static int TypeSso = 1;
    public static int TypeOrgPru = 2;

    public static Dictionary<int, string> ListTypeCustomer = new Dictionary<int, string>()
    {
        {1,"sso"},
        {2,"PRUgift cung cấp"},
    };
    
    public static Dictionary<int, string> ListTypeCustomerColor = new Dictionary<int, string>()
    {
        {1,"<span class='badge badge-success'>sso</span>"},
        {2,"<span class='badge badge-primary'>PRUgift cung cấp</span>"},
    };
    
    public static Dictionary<int, string> ListStatusCustomer = new Dictionary<int, string>()
    {
        {0,"Chưa kích hoạt"},
        {1,"Hoạt động"},
    };
    
    public static Dictionary<int, string> ListStatusCustomerColor = new Dictionary<int, string>()
    {
        {1,"<span class='badge badge-success'>Hoạt động</span>"},
        {0,"<span class='badge badge-warning'>Chưa kích hoạt</span>"},
    };
}