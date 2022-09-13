using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using CMS.Services.Files;

namespace CMS.Extensions.Validate
{
    public class ValidMaxFileSizeAttribute : ValidationAttribute
    {
        private long? _maxFileSize;

        public ValidMaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var iConfiguration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));
            if (_maxFileSize == 0)
            {
                _maxFileSize = iConfiguration?.GetSection("AppSetting").GetValue<int>("MaxUploadSize") * 1024 * 1024;
            }
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
#pragma warning disable CS0612 // Type or member is obsolete
            return $"Upload file tối đa {FileService.SizeConverter(_maxFileSize ?? 0)}.";
#pragma warning restore CS0612 // Type or member is obsolete
        }
    }
}
