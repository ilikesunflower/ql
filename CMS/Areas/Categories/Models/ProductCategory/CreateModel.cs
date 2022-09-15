using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;

namespace CMS.Areas.Categories.Models.ProductCategory;

public class CreateModel
{
    [MaxLength(255,ErrorMessage = "Vị trí chỉ được phép chứa 255 ký tự!")]
    [ValidXss]
    [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
    public string Name { get; set; }
     
    // [MaxLength(250,ErrorMessage = "Font chỉ được phép chứa 255 ký tự!")]
    // [Required(ErrorMessage = "Vui lòng nhập Font.")]
    [ValidXss]
    public string Font { get; set; }
    
    [MaxLength(255,ErrorMessage = "Tên ảnh chỉ được phép chứa 255 ký tự!")]
    [Required(ErrorMessage = "Vui lòng nhập Ảnh.")]
    [ValidXss]
    public string ImageBanner { get; set; }
    public string ImageBannerMobile { get; set; }
    
    [Range(0, 99999999999, ErrorMessage = "Vui lòng nhập thứ tự lớn hơn 0.")]
    public int? Ord { get; set; }
    public int? Pid { get; set; }
    public List<CMS_EF.Models.Products.ProductCategory> ListCategories { get; set; }

}