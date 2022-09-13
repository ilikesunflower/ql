using CMS.Extensions.Validate;
using CMS_EF.Models.Articles;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Areas.Categories.Models.Article
{
    public class CreateModel
    {
        [MaxLength(255,ErrorMessage = "Tiêu đề chỉ được phép chứa 255 ký tự!")]
        [ValidXss]
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
        public string Title { get; set; }
        
        [ValidScript]
        [Required(ErrorMessage = "Vui lòng nhập mô tả.")]
        public string Lead { get; set; }
        
        [ValidScript]
        [Required(ErrorMessage = "Vui lòng nhập nội dung.")]
        public string Detail { get; set; }
     
        [MaxLength(1000)]
        [ValidXss]
        [Required(ErrorMessage = "Vui lòng nhập ảnh.")]
        public string Thumbnail { get; set; }

        public int? Status { get; set; }
        public bool IsHot { get; set; }

        [MaxLength(300)]
        [ValidXss]
        public string Author { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại bài viết.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn loại bài viết.")]
        public int ArticleType { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày đăng.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [ValidThanDayAttribute]
        [ValidXss]
        public string PostDate { get; set; }

        public List<ArticleType> ListArticleType { get; set; }

        public bool StatusBox { get; set; }


    }
}
