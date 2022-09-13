using System;
using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;

namespace CMS.Areas.Admin.ViewModels.Personal
{
    public class EditProfileViewModel
    {
        public int Id { get; set; }
        [ValidXss] public string UserName { get; set; }


        [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường họ tên")]
        [ValidXss]
        public string FullName { get; set; }

        [ValidXss] public string Image { get; set; }

        [ValidXss] public string Email { get; set; }
        
        // [Phone(ErrorMessage = "Không đúng định dạng số điện thoại, vui lòng nhập lại")]
        // [RegularExpression(@"^(\d{10,11})$", ErrorMessage = "Không đúng định dạng số điện thoại, vui lòng nhập lại")]
        [ValidXss]
        public String PhoneNumber { get; set; }

        public int Sex { get; set; }
    }
}