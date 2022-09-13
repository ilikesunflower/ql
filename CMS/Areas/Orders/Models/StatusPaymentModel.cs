using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;
namespace CMS.Areas.Orders.Models;

public class StatusPaymentModel
{
    [Required(ErrorMessage = "Vui lòng nhập mã đơn hàng")]
    [ValidXss]
    public string OrderCode { get; set; }
    public int Status { get; set; }
}