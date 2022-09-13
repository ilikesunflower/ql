using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace CMS.Extensions.Validate
{
    public class ValidAllowedFileExtensionsDocumentAttribute : ValidationAttribute
    {
        private string[] _extensions;

        public ValidAllowedFileExtensionsDocumentAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var iConfiguration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));
            if (_extensions == null)
            {
                _extensions = iConfiguration?.GetSection("AppSetting").GetSection("AllowedFileDocument").Get<string[]>();
            }

            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Hệ thống chỉ hỗ trợ các định dạng: {String.Join(",", _extensions)}";
        }
    }
}
