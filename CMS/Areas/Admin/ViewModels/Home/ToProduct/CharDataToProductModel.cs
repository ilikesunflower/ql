using System.Collections.Generic;
using CMS.Extensions.Validate;

namespace CMS.Areas.Admin.ViewModels.Home.ToProduct;

public class CharDataToProductModel
{
    public List<string> Categories { get; set; }
    public List<double> Prices { get; set; }
    
    [ValidXss]
    public string FilterStatus { get; set; }
    
    [ValidXss]
    public string ValueSuffix { get; set; }
}