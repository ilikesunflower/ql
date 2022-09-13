using CMS.Extensions.Validate;
using CMS_Access.Repositories;
using CMS_EF.Models;
using Ganss.XSS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Areas.Admin.ViewModels.Menus
{
    public class CreateViewModel
    {
        public int Id { get; set; }

        public int Pid { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường tên menu")]
        [ValidXss]
        // [NameFormCreateValidation]
        public string Name { get; set; }
        public int Status { get; set; }

        [ValidXss]
        public string CssClass { get; set; }

        [ValidXss]
        public string Url { get; set; }

        public int? ControllerId { get; set; }

        public int? ActionId { get; set; }

        public List<Menu> ListMenus { get; set; }

        public List<CMS_EF.Models.Identity.ApplicationController> ListControllers { get; set; }
    }
    public class NameFormCreateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as CreateViewModel;
            var context = (IMenuRepository)validationContext.GetService(typeof(IMenuRepository));
            var iHtmlSanitizer = (IHtmlSanitizer)validationContext.GetService(typeof(IHtmlSanitizer));
            if (!string.IsNullOrEmpty(model?.Name))
            {
                var checkAny = context?.FindByName(iHtmlSanitizer?.Sanitize(model.Name.Trim()));
                if (checkAny != null)
                {
                    return new ValidationResult("Menu đã tồn tại, vui lòng nhập tên menu khác");
                }
            }
            return null;
        }
    }
}
