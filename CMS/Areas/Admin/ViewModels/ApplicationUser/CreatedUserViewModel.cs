using Castle.Core.Internal;
using CMS.Extensions.Validate;
using CMS_Access.Repositories;
using Ganss.XSS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Areas.Admin.ViewModels.ApplicationUser
{
    public class CreatedUserViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường tài khoản")]
        [ValidXss]
        [RegularExpression(@"^[a-zA-Z0-9?><;.,{}[\]\-_+=!@#$%\^&*|']*$", ErrorMessage = "Bạn nhập sai định dạng tài khoản")]
        [UserNameCustomValidation]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự", MinimumLength = 8)]
        [ValidXss]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W]).{8,}$", ErrorMessage = "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt, 1 ký tự viết hoa, 1 ký tự viết thường, 1 ký tự số, ví dụ @Test2021")]
        // [DataType(DataType.Password)]
        public string Password { get; set; }

        // [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và Nhập lại mật khẩu không trùng khớp.")]
        [ValidXss]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường họ tên")]
        [ValidXss]
        public string FullName { get; set; }

        [ValidXss]
        public string Address { get; set; }

        [ValidXss]
        public string Image { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường email")]
        [EmailAddress(ErrorMessage = "Không đúng định dạng của email, vui lòng nhập lại")]
        // [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Không đúng định dạng của email, vui lòng nhập lại")]
        [ValidXss]
        [EmailCreateCustomValidation]
        public string Email { get; set; }

        // [Phone(ErrorMessage = "Không đúng định dạng số điện thoại, vui lòng nhập lại")]
        // [RegularExpression(@"^(\d{10,11})$", ErrorMessage = "Không đúng định dạng số điện thoại, vui lòng nhập lại")]
        [ValidXss]
        public string PhoneNumber { get; set; }

        public int Sex { get; set; }
        public int IsActive { get; set; }

        public int? TypeUser { get; set; }

        // [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường ngày sinh")]
        // [DataType(DataType.Date)]
        // [ValidXss]
        // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        // [ValidThanDayAttribute]
        // public string BirthDay { get; set; }

        public List<RoleInput> ListRoles { get; set; }

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class UserNameCustomValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (CreatedUserViewModel)validationContext.ObjectInstance;
            if (!model.UserName.IsNullOrEmpty())
            {
                var iApplicationUserRepository = (IApplicationUserRepository)validationContext.GetService(typeof(IApplicationUserRepository));
                var iHtmlSanitizer = (IHtmlSanitizer)validationContext.GetService(typeof(IHtmlSanitizer));
                var checkAny = iApplicationUserRepository?.FindByUsername(iHtmlSanitizer?.Sanitize(model.UserName.Trim()));
                if (checkAny != null)
                {
                    return new ValidationResult("Tài khoản đã tồn tại trong hệ thống, vui lòng nhập tài khoản khác");
                }
            }
            else
            {
                return new ValidationResult("Vui lòng nhập đầy thông tin vào trường tài khoản");
            }
            return ValidationResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class EmailCreateCustomValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (CreatedUserViewModel)validationContext.ObjectInstance;
            if (!model.Email.IsNullOrEmpty())
            {
                var iApplicationUserRepository = (IApplicationUserRepository)validationContext.GetService(typeof(IApplicationUserRepository));
                var iHtmlSanitizer = (IHtmlSanitizer)validationContext.GetService(typeof(IHtmlSanitizer));
                var checkAny = iApplicationUserRepository?.FindByEmail(iHtmlSanitizer?.Sanitize(model.Email.Trim()));
                if (checkAny != null)
                {
                    return new ValidationResult("Email đã tồn tại trong hệ thống, vui lòng nhập email khác");
                }
            }
            else
            {
                return new ValidationResult("Vui lòng nhập đầy thông tin vào trường email");
            }
            return ValidationResult.Success;
        }
    }
}
