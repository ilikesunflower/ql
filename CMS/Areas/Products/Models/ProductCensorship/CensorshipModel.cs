using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;

namespace CMS.Areas.Products.Models.ProductCensorship;

public class CensorshipModel
{
    [Required(ErrorMessage = "Vui lòng nhập mã hàng.")]
    public int ProductId { set; get; }  
    
    [Required(ErrorMessage = "BackUrl lỗi")]
    public string BackUrl { set; get; }
    
    [ValidXss]
    [Required(ErrorMessage = "Vui lòng nhập nội dung.")]
    [MaxLength(250,ErrorMessage = "Nội dung phải nhỏ hơn 250 kí tự!")]
    public string Comment { set; get; }
}