namespace CMS_App_Api.Areas.Orders.Const;

public class CustomerPointLogStatus
{
    public static CustomerPointLogStatus Use => new(0, "Use");
    public static CustomerPointLogStatus Revert => new(1, "Revert");
    
    public CustomerPointLogStatus(int status, string name)
    {
        Status = status;
        Name = name;
    }

    public int Status { set; get; } 
    public string Name { set; get; } 
}