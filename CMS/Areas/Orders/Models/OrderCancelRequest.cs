using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;

namespace CMS.Areas.Orders.Models;

public class OrderCancelRequest
{
    [Required(ErrorMessage = "Vui lòng nhập mã đơn hàng")]
    [ValidXss]
    public string Id { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập lý do hủy đơn")]
    [ValidXss]
    [StringLength(255, ErrorMessage = "Vui lòng nhập tối đa 225 ký tự", MinimumLength = 1)]
    public string Note { get; set; }
}