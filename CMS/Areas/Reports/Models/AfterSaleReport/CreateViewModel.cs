using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;
using Microsoft.AspNetCore.Http;

namespace CMS.Areas.Reports.Models.AfterSaleReport;

public class CreateViewModel
{
    [MaxLength(255,ErrorMessage = "Tên báo cáo chỉ được phép chứa 255 ký tự!")]
    [ValidXss]
    [Required(ErrorMessage = "Vui lòng nhập tên báo cáo.")]
    public string Name { get; set; }
    
    [ValidXss]
    public string Month { get; set; }
    
    [ValidXss]
    public string  Quater { get; set; }
    
    [ValidXss]
    public string Year { get; set; }
    
    [Required(ErrorMessage = "Vui lòng chọn loại báo cáo.")]
    public int Type { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập file báo cáo")]
    public IFormFile File { get; set; }
}