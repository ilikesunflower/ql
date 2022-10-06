using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CMS.Areas.Admin.ViewModels.Home.ToProduct;

public class ToProductViewModel
{
    public int FilterStatus { get; set; }
    [ValidXss]
    public string DateStart { get; set; }
    [ValidXss]
    public string DateEnd { get; set; }

}