using CMS.Extensions.Validate;

namespace CMS.Areas.Admin.ViewModels.Home.Sales;

public class SalesFilterViewModel
{
    [ValidXss]
    public string TimeFlow { get; set; }
    [ValidXss]
    public string DateStart { get; set; }
    [ValidXss]
    public string DateEnd { get; set; }
}