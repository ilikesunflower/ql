using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;

namespace CMS.Areas.Admin.ViewModels.Home.ToProduct;

public class ToProductViewModel
{
    [RegularExpression(@"^[1-9]*$", ErrorMessage = "Vui lòng nhập số.")]
    public int FilterStatus { get; set; }

    [ValidXss] public string DateStart { get; set; }
    [ValidXss] public string DateEnd { get; set; }
}