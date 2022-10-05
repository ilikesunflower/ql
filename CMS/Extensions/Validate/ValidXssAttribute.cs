using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Net;
using CMS_Lib.Helpers;
using CMS_Lib.Util;

namespace CMS.Extensions.Validate
{
    public class ValidXssAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && CollectionHelper.IsList(value))
            {
                foreach (var item in (IEnumerable)value)
                {
                    if (item != null && item.GetType() == (typeof(string)))
                    {
                        var data = $"{item}";
                        data = WebUtility.UrlDecode(data);
                        if (!string.IsNullOrEmpty(data) &&
                            (data.ToLower().Contains("<script>") || data.ToLower().Contains("</script>")))
                        {
                            return new ValidationResult(
                                "Hệ thống không hỗ trợ nhập script cho nội dung này, vui lòng bỏ script");
                        }
                        else if (!string.IsNullOrEmpty(data) && CmsFunction.IsHtml(data.TrimEnd()))
                        {
                            return new ValidationResult(
                                "Hệ thống không hỗ trợ nhập html cho nội dung này, vui lòng bỏ html");
                        }
                        else if (!string.IsNullOrEmpty(data) && HtmlSanitizerHelper.IsXss(data))
                        {
                            return new ValidationResult("Hệ thống không hỗ trợ nội dung này");
                        }else if (!string.IsNullOrEmpty(data))
                        {
                            foreach (var t in HtmlSanitizerHelper.ListTagXss)
                            {
                                if (data.ToLower().Contains(t))
                                {
                                    return new ValidationResult("Hệ thống không hỗ trợ nội dung này");
                                }
                            }
                        }
                    }
                }
            }
            else if (value != null && value.GetType() == (typeof(string)))
            {
                var data = $"{value}";
                data = WebUtility.UrlDecode(data);
                if (!string.IsNullOrEmpty(data) &&
                    (data.ToLower().Contains("<script>") || data.ToLower().Contains("</script>")))
                {
                    return new ValidationResult(
                        "Hệ thống không hỗ trợ nhập script cho nội dung này, vui lòng bỏ script");
                }
                else if (!string.IsNullOrEmpty(data) && CmsFunction.IsHtml(data.TrimEnd()))
                {
                    return new ValidationResult("Hệ thống không hỗ trợ nhập html cho nội dung này, vui lòng bỏ html");
                }
                else if (!string.IsNullOrEmpty(data) && HtmlSanitizerHelper.IsXss(data))
                {
                    return new ValidationResult("Hệ thống không hỗ trợ nội dung này");
                }
                else if (!string.IsNullOrEmpty(data))
                {
                    foreach (var t in HtmlSanitizerHelper.ListTagXss)
                    {
                        if (data.ToLower().Contains(t))
                        {
                            return new ValidationResult("Hệ thống không hỗ trợ nội dung này");
                        }
                    }
                }
            }

            return null;
        }
    }


    public class ValidScriptAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && value.GetType().ImplementsGenericInterface(typeof(string)))
            {
                var data = $"{value}";
                data = WebUtility.UrlDecode(data);
                if (!string.IsNullOrEmpty(data) &&
                    (data.ToLower().Contains("<script") || data.ToLower().Contains("</script>")))
                {
                    return new ValidationResult(
                        "Hệ thống không hỗ trợ nhập script cho nội dung này, vui lòng bỏ script");
                }
                else if (!string.IsNullOrEmpty(data))
                {
                    foreach (var t in HtmlSanitizerHelper.ListTagXssScript)
                    {
                        if (data.ToLower().Contains(t))
                        {
                            return new ValidationResult("Hệ thống không hỗ trợ nội dung này");
                        }
                    }
                }
            }

            return null;
        }
    }

    public class ValidFullPathAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && value.GetType().ImplementsGenericInterface(typeof(string)))
            {
                var data = $"{value}";
                data = WebUtility.UrlDecode(data);
                if (!string.IsNullOrEmpty(data) && data.StartsWith("../"))
                {
                    return new ValidationResult("Hệ thống không hỗ trợ nhập nội dung này, vui lòng nhập lại");
                }
            }

            return null;
        }
    }
}