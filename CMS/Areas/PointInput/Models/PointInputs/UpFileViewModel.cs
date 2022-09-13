using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;
using Microsoft.AspNetCore.Http;

namespace CMS.Areas.PointInput.Models.PointInputs;

public class UpFileViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn file!")]
    [ValidExcel]
    [ValidMaxFileSize(0)]
    public IFormFile File { get; set; }
}