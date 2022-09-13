using CMS.Extensions.Validate;
using CMS_EF.Models.Articles;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Areas.Categories.Models.Banner
{
    public class CreateModel
    {
        [MaxLength(250,ErrorMessage = "Vị trí chỉ được phép chứa 255 ký tự!")]
        [ValidXss]
        [Required(ErrorMessage = "Vui lòng chọn vị trí.")]
        public string Alias { get; set; }
        
        [ValidXss]
        public string Link { get; set; }  
     
        [MaxLength(1000)]
        [Required(ErrorMessage = "Vui lòng nhập ảnh.")]
        [ValidXss]
        public string Images { get; set; }
        public string ImagesMobile { get; set; }

        public int Status { get; set; }
        
        [Range(0, 99999999999, ErrorMessage = "Vui lòng nhập thứ tự lớn hơn 0.")]
        public int? Ord { get; set; }
        public Dictionary<int, string> ListBanner { get; set; }
    }
}
