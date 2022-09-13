using System.ComponentModel.DataAnnotations;
using CMS.Extensions.Validate;
using Microsoft.AspNetCore.Http;

namespace CMS.Areas.Customer.Models.Customer;

public class ImportCustomerViewModel
{
    [Required]
    [ValidExcel]
    [ValidMaxFileSize(0)]
    public IFormFile File { get; set; }
}