using System.Collections.Generic;

namespace CMS.Areas.Admin.ViewModels.Home.OrderDetail;

public class OrderDetailViewModel
{
    public string Name { get; set; }
    public int CountOrder { get; set; }
    public int Type { get; set; }
    public double PriceOrder { get; set; }
    public List<int> ListStatus { get; set; }
    public string Date { get; set; }
}