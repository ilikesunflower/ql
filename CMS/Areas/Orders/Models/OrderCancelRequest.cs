using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;
using NPOI.SS.Formula.Functions;

namespace CMS.Areas.Orders.Models;

public class OrderCancelRequest
{
    [Required(ErrorMessage = "Vui lòng nhập mã đơn hàng")]
    [ValidXss]
    public string Id { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập lý do hủy đơn")]
    public int Note { get; set; }
}