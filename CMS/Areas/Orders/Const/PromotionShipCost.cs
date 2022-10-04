using System.Collections.Generic;

namespace CMS.Areas.Orders.Const;

public class PromotionShipCost
{
    public static List<PromotionModel> ListPromotionShipConst = new()
    {
        new PromotionModel{ TotalStart= 500000, CodeAddress = "70", Percent= 100 },
        new PromotionModel{ TotalStart= 2000000,TotalEnd = 5000000,  Percent= 20 },
        new PromotionModel{ TotalStart= 5000000, TotalEnd= 10000000, Percent= 30 },
        new PromotionModel{ TotalStart= 10000000, TotalEnd= 20000000, Percent= 50 },
        new PromotionModel{ TotalStart= 20000000,Percent= 100 }
    };
}


public class PromotionModel
{
    public int TotalStart { get; set; }
    public int? TotalEnd { get; set; }
    public string CodeAddress { get; set; }
    public int Percent { get; set; }
}

