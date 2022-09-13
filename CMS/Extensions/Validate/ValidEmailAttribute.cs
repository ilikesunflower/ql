using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace CMS.Extensions.Validate
{
    public class ValidEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var data = value?.ToString();
            if (!string.IsNullOrEmpty(data))
            {
                if (!IsValidEmail(data))
                {
                    return new ValidationResult("Vui lòng nhập đúng định dạng email");
                }
            }
            return null;
        }
        private bool IsValidEmail(string email)
        {
            var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
            return !String.IsNullOrEmpty(email) && r.IsMatch(email);
        }
    }
}
