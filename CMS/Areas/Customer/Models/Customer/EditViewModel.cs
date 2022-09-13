using CMS.Areas.Admin.ViewModels.ApplicationUser;
using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;

namespace CMS.Areas.Customer.Models.Customer;
public class EditViewModel
{
    public string UserName { get; set; }
    public int Type { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường email")]
    [EmailAddress(ErrorMessage = "Không đúng định dạng của email, vui lòng nhập lại")]
    // [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Không đúng định dạng của email, vui lòng nhập lại")]
    [ValidXss]
    public string Email { get; set; }
    
    [ValidXss]
    [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường họ tên")]
    public string FullName { get; set; }
    
    // [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường số điện thoại")]
    // [Phone(ErrorMessage = "Không đúng định dạng số điện thoại, vui lòng nhập lại")]
    // [RegularExpression(@"^(\d{10,11})$", ErrorMessage = "Không đúng định dạng số điện thoại, vui lòng nhập lại")]
    [ValidXss]
    public string Phone { get; set; }
    
    [ValidXss]
    public string Detail { get; set; }
    
    [ValidXss]
    public string Org { get; set; }
    public int Status { get; set; }
    public int TypeGroup { get; set; }
}