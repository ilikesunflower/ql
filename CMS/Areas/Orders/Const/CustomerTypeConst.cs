namespace CMS.Areas.Orders.Const;

public class CustomerTypeConst
{
    public static CustomerTypeConst NormalCustomer => new CustomerTypeConst(1,"Tài khản thông thường");
    public static CustomerTypeConst PrudentialCustomer => new CustomerTypeConst(2,"Tài khản phòng ban");
    
    public int Type { set; get; }
    public string Name { set; get; }
    
    public CustomerTypeConst(int type, string name)
    {
        Type = type;
        Name = name;
    }
}