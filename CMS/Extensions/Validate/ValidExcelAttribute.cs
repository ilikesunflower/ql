using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Extensions.Validate
{

    public class ValidExcelAttribute : ValidationAttribute
    {
        private readonly List<string> _extentions = new() { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var result = ValidationResult.Success;
            if (value is IFormFile file)
            {
                if (!_extentions.Contains(file.ContentType))
                {
                    result = new ValidationResult("Hệ thống chỉ hỗ trợ file .xlsx");
                }
            }

            return result;
        }
    }
}
