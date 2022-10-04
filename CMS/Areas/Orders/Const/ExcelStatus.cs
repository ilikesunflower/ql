namespace CMS.Areas.Orders.Const;

public class ExcelStatus
{
    private ExcelStatus(int status, string statusStr)
    {
        Status = status;
        StatusStr = statusStr;
    }
    public static ExcelStatus Unpaid => new(0,"Chưa thanh toán");

    public static ExcelStatus Paid => new(1,"Đã thanh toán");
    
    public int Status { set; get; }
    public string StatusStr { set; get; }
}