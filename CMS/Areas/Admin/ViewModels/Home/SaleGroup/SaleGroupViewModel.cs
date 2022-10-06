using CMS.Extensions.Validate;

namespace CMS.Areas.Admin.ViewModels.Home.SaleGroup;

public class SaleGroupViewModel
{
    [ValidXss] public string DateStart { get; set; }
    [ValidXss] public string DateEnd { get; set; }
}