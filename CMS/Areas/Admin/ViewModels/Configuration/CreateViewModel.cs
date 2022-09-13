using CMS.Extensions.Validate;
using CMS_Access.Repositories;
using Ganss.XSS;
using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Areas.Admin.ViewModels.Configuration
{
    public class CreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường tên")]
        [ValidXss]
        public String Name { get; set; }

        [ValidXss]
        [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường alias")]
        [NameFormCreateValidation]
        public String Val { get; set; }

        [ValidScript]
        public String Detail { get; set; }
    }
    public class NameFormCreateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as CreateViewModel;
            var context = (IConfigurationRepository)validationContext.GetService(typeof(IConfigurationRepository));
            var iHtmlSanitizer = (IHtmlSanitizer)validationContext.GetService(typeof(IHtmlSanitizer));
            if (!string.IsNullOrEmpty(model?.Val))
            {
                var checkAny = context?.FindByVal(iHtmlSanitizer?.Sanitize(model.Val.Trim()));
                if (checkAny != null)
                {
                    return new ValidationResult("Alias đã tồn tại, vui lòng nhập alias khác");
                }
            }

            return null;
        }
    }
}
