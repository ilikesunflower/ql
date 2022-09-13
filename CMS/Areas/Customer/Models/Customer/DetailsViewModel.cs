using System.Collections.Generic;
using CMS_EF.Models.Customers;

namespace CMS.Areas.Customer.Models.Customer;

public class DetailsViewModel
{
    public CMS_EF.Models.Customers.Customer Customer { get; set; }
    
    public bool? IsResetPass { get; set; }
    
    public List<CustomerCoupon> ListCustomerCoupons { get; set; }
}