using System;
using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;
using CMS_Access.Repositories.Customers;
using Ganss.XSS;

namespace CMS.Areas.Customer.Models.Customer;

public class CreateViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường tài khoản")]
    [ValidXss]
    [RegularExpression(@"^[a-zA-Z0-9?><;.,{}[\]\-_+=!@#$%\^&*|']*$", ErrorMessage = "Bạn nhập sai định dạng tài khoản")]
    [UserNameCustomValidation]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường email")]
    [EmailAddress(ErrorMessage = "Không đúng định dạng của email, vui lòng nhập lại")]
    // [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Không đúng định dạng của email, vui lòng nhập lại")]
    [ValidXss]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường họ tên")]
    [ValidXss]
    public string FullName { get; set; }
    
    // [Required(ErrorMessage = "Vui lòng nhập thông tin vào trường số điện thoại")]
    // [Phone(ErrorMessage = "Không đúng định dạng số điện thoại, vui lòng nhập lại")]
    // [RegularExpression(@"^(\d{10,11})$", ErrorMessage = "Không đúng định dạng số điện thoại, vui lòng nhập lại")]
    [ValidXss]
    public string Phone { get; set; }
    
    [ValidXss]
    public string Org { get; set; }
    
    [Required(ErrorMessage = "Vui lòng chon đối tượng khách hàng")]
    public int TypeGroup { get; set; }
    
    [ValidXss]
    public string Detail { get; set; }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class UserNameCustomValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var model = (CreateViewModel)validationContext.ObjectInstance;
        if (!string.IsNullOrEmpty(model.UserName))
        {
            var iCustomerRepository = (ICustomerRepository)validationContext.GetService(typeof(ICustomerRepository));
            var iHtmlSanitizer = (IHtmlSanitizer)validationContext.GetService(typeof(IHtmlSanitizer));
            var checkAny = iCustomerRepository?.FindByUserName(iHtmlSanitizer?.Sanitize(model.UserName.Trim()));
            if (checkAny != null)
            {
                return new ValidationResult("Tài khoản khách hàng đã tồn tại trong hệ thống, vui lòng nhập tài khoản khác");
            }
        }
        else
        {
            return new ValidationResult("Vui lòng nhập đầy thông tin vào trường tài khoản");
        }
        return ValidationResult.Success;
    }
}