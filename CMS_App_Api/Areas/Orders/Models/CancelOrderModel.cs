using System.ComponentModel.DataAnnotations;

namespace CMS_App_Api.Areas.Orders.Models;

public class CancelOrderModel
{
    [Required(ErrorMessage = "Vui lòng nhập lý do")]
    [StringLength(255, ErrorMessage = "Lý do chỉ được dài trong khoảng từ {2} đến {1} ký tự",MinimumLength = 2)]
    public string Message{ get; set; }
}