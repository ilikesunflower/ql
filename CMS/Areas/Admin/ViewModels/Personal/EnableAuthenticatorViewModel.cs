using System.ComponentModel.DataAnnotations;

namespace CMS.Areas.Admin.ViewModels.Personal
{
    public class EnableAuthenticatorViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã xác thực")]
        [StringLength(7, ErrorMessage = "Mã xác thực phải lớn hơn hoặc bằng {2} và nhỏ hơn {1} kí tự.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }

        public string AuthenticatorUri { get; set; }
    }
}
