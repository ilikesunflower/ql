using CMS.Extensions.Validate;
using CMS_Access.Repositories;
using Ganss.XSS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Areas.Admin.ViewModels.ApplicationRole
{
    public class EditApplicationRoleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập đầy thông tin vào trường tên nhóm quyền")]
        [ValidXss]
        [NameRoleFormEditValidation]
        public string Name { get; set; }

        [ValidXss]
        public string Description { get; set; }

        public List<ExtendRoleController> ListRoleControllerAction { get; set; }

    }

    public class NameRoleFormEditValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as EditApplicationRoleViewModel;
            var context = (IApplicationRoleRepository)validationContext.GetService(typeof(IApplicationRoleRepository));
            var iHtmlSanitizer = (IHtmlSanitizer)validationContext.GetService(typeof(IHtmlSanitizer));
            if (!string.IsNullOrEmpty(model?.Name))
            {
                var role = context?.FindByName(iHtmlSanitizer?.Sanitize(model.Name.Trim()));
                if (role != null && role.Id != model.Id)
                {
                    return new ValidationResult("Tên nhóm quyền đã tồn tại trong hệ thống, vui lòng nhập tên khác");
                }
            }
            return null;
        }
    }
}
