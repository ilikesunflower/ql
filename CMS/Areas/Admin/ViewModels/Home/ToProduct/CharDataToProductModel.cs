using System.Collections.Generic;

namespace CMS.Areas.Admin.ViewModels.Home.ToProduct;

public class CharDataToProductModel
{
    public List<string> Categories { get; set; }
    public List<double> Prices { get; set; }
    public string FilterStatus { get; set; }
    public string ValueSuffix { get; set; }
}