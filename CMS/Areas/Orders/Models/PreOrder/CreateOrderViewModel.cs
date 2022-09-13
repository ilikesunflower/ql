using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;

namespace CMS.Areas.Orders.Models.PreOrder;

public class CreateOrderViewModel
{
    [ValidXss]
    [Required(ErrorMessage = "Vui lòng nhập mã PreOrder.")]
    public int PreOrderId { set; get; }
    
    // [ValidXss]
    // public double? Price { set; get; }
    
    [ValidXss]
    public int? Point { set; get; }
    
    [ValidXss]
    [MaxLength(250,ErrorMessage = "Mã phải nhỏ hơn 250 kí tự!")]
    public string Coupon { set; get; }
}