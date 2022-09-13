using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Ganss.XSS;

namespace CMS.Extensions.Validate
{
    public class ValidXssAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var data = value?.ToString();
            if (data != null && (data.ToLower().Contains("<script>") || data.ToLower().Contains("</script>")))
            {
                return new ValidationResult("Hệ thống không hỗ trợ nhập script cho nội dung này, vui lòng bỏ script");
            }
            else if (data != null && ContainsHTML(data.TrimEnd()))
            {
                return new ValidationResult("Hệ thống không hỗ trợ nhập html cho nội dung này, vui lòng bỏ html");
            }
            return null;
        }
        private bool ContainsHTML(string checkString)
        {
            return Regex.IsMatch(checkString, "<(.|\n)*?>");
        }
    }

    public class ValidScriptAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var data = value?.ToString();
            if (data != null && (data.ToLower().Contains("<script") || data.ToLower().Contains("</script>")))
            {
                return new ValidationResult("Hệ thống không hỗ trợ nhập script cho nội dung này, vui lòng bỏ script");
            }
            return null;
        }
    }
}
