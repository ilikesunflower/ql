using CMS.Extensions.Validate;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Areas.Admin.ViewModels.Home.Sales;

public class SalesFilterViewModel
{
    [ValidXss] [BindProperty] public string TimeFlow { get; set; }
    [ValidXss] [BindProperty] public string DateStart { get; set; }
    [ValidXss] [BindProperty] public string DateEnd { get; set; }
}