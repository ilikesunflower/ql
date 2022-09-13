using System.Collections.Generic;
using AngleSharp;
using CMS.Services.Token;
using CMS_EF.Models.Customers;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Coupons.Models;

public class IndexCouponModel
{
    public int Page { get; set; }
    public PagingList<HistoryFileCoupon> ListData { get; set; }
    public ITokenService TokenService { get; set; } 
    public bool IsImport { get; set; } 
        
}