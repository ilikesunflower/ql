using System.Collections.Generic;
using CMS_EF.Models.Customers;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Coupons.Models;

public class DetailCouponModel
{
    public HistoryFileCoupon HistoryFile { get; set; }
    public PagingList<CustomerCoupon> ListCustomerCoupon { get; set; }
    public int Page { get; set; }
    public bool IsSendNotification { get; set; }
}