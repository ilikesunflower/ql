using CMS.Extensions.Validate;
using CMS_Access.Repositories;
using Ganss.XSS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Areas.Admin.ViewModels.ApplicationRole
{
    public class CreateApplicationRoleViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập đầy thông tin vào trường tên nhóm quyền")]
        [ValidXss]
        [NameRoleExitsValidation]
        public string Name { get; set; }

        [ValidXss]
        public string Description { get; set; }

        [ValidXss]
        public List<ExtendRoleController> ListRoleControllerAction { get; set; }
    }

    public class NameRoleExitsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as CreateApplicationRoleViewModel;
            var context = (IApplicationRoleRepository)validationContext.GetService(typeof(IApplicationRoleRepository));
            var iHtmlSanitizer = (IHtmlSanitizer)validationContext.GetService(typeof(IHtmlSanitizer));
            if (!string.IsNullOrEmpty(model?.Name))
            {
                var checkAny = context?.FindByName(iHtmlSanitizer?.Sanitize(model.Name.Trim()));
                if (checkAny != null)
                {
                    return new ValidationResult("Tên nhóm quyền đã tồn tại trong hệ thống, vui lòng nhập tên khác");
                }
            }
            return null;
        }
    }
    
    
    public class ExtendRoleAction
    {
        public int Id { get; set; }
        [ValidXss]
        public string Title { get; set; }
        [ValidXss]
        public string Name { get; set; }
        public int ControllerId { get; set; }
        public bool IsChecked { get; set; }
    }
    public class ExtendRoleController
    {
        public int Id { get; set; }
        [ValidXss]
        public string Name { get; set; }
        [ValidXss]

        public string Title { get; set; }

        public List<ExtendRoleAction> ListAction { get; set; }

    }
}
