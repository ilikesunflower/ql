using CMS.Extensions.Validate;

namespace CMS.Areas.Admin.ViewModels.Home.ToProduct;

public class ToProductViewModel
{
    public int FilterStatus { get; set; }
    [ValidXss]
    public string DateStart { get; set; }
    [ValidXss]
    public string DateEnd { get; set; }
}