using System.Collections.Generic;
using CMS_EF.Models.Customers;

namespace CMS.Areas.Orders.Models.PreOrder;

public class DetailPreOrderViewModel
{
    public CMS_EF.Models.PreOrders.PreOrder PreOrder { get; set; }
    public List<CustomerCoupon> Coupons { get; set; } = new();
    public double Point { get; set; }
}