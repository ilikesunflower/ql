using CMS.Extensions.Validate;
using System.ComponentModel.DataAnnotations;

namespace CMS.Areas.Admin.ViewModels.Personal
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        [DataType(DataType.Password)]
        [ValidXss]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W]).{8,}$", ErrorMessage = "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt, 1 ký tự viết hoa, 1 ký tự viết thường, ví dụ @Test2021")]
        [ValidXss]
        public string PasswordNew { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu mới")]
        [DataType(DataType.Password)]
        [Compare("PasswordNew", ErrorMessage = "Nhập lại mật khẩu mới không khớp")]
        [ValidXss]
        public string PasswordNewOld { get; set; }
    }
}
