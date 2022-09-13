using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;
using Microsoft.AspNetCore.Http;

namespace CMS.Areas.Coupons.Models;

public class DataExportViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn file!")]
    [ValidExcel]
    [ValidMaxFileSize(0)]
    public IFormFile File{ get; set; }
}