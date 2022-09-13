using System.Collections.Generic;

namespace CMS.Areas.Products.Const;

public class ProductCensorshipConst
{
    public static ProductCensorshipConst Pending => new(0,"Chờ duyệt");
    public static ProductCensorshipConst Approved => new(1,"Đã duyệt");
    public static ProductCensorshipConst NotApproved => new(2,"Trả về");

    public static readonly List<ProductCensorshipConst> ListStatus = new()
    {
        Pending,Approved,NotApproved
    };

    private ProductCensorshipConst(int status, string name)
    {
        Status = status;
        Name = name;
    }

    public int Status { get; }
    public string Name { get; }
}