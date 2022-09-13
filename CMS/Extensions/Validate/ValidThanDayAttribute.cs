using CMS_Lib.Util;
using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Extensions.Validate
{
    public class ValidThanDayAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var data = value?.ToString();
            if (data != null)
            {
                var convertStringToDateTime = CmsFunction.ConvertStringToDateTime(data);
                if(convertStringToDateTime == null)
                {
                    convertStringToDateTime = CmsFunction.ConvertStringToDateTimeH(data).GetValueOrDefault();

                }
                if (convertStringToDateTime > DateTime.Today.AddDays(1))
                {
                    return new ValidationResult("Bạn chọn ngày lớn hơn ngày hiện tại, vui lòng chọn lại");
                }
            }
            return null;
        }
    }
}
