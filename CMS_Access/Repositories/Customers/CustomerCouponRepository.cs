using System;
using System.Collections.Generic;
using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories.Customers;

public interface ICustomerCouponRepository : IBaseRepository<CustomerCoupon>, IScoped
{
    CustomerCoupon? FindByCustomerId(string coupon, int id);
    CustomerCoupon FindCouponActiveByCustomerId(int customerId, string couponCode);
    CustomerCoupon FindCouponActiveEdit(int customerId, string couponCode, string couponCodeOld);
    CustomerCoupon FindCouponInActiveByCustomerId(string ordersCouponCode, int? ordersCustomerId);
    List<CustomerCoupon> GetALlByCustomerId(int? preOrderCustomerId);
    List<CustomerCoupon> GetALlByCustomerIdOrderId(int? customerId, string voucherOrder);
    IQueryable<CustomerCoupon> GetAllNoDateByCustomerId(int customerId);
}
public class CustomerCouponRepository :BaseRepository<CustomerCoupon>, ICustomerCouponRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public CustomerCouponRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

    public CustomerCoupon? FindByCustomerId(string couponCode, int customerId)
    {
        DateTime timeNow = DateTime.Now;
        var que = _applicationDbContext.CustomerCoupon.Where(x =>
            x.StartTimeUse <= timeNow && x.EndTimeUse >= timeNow && x.Status == 0 &&
            x.Flag == 0 && x.CustomerId == customerId && x.Code == couponCode).FirstOrDefault();
        return que!;
    }
    
    public CustomerCoupon FindCouponActiveByCustomerId(int customerId, string couponCode)
    {
        DateTime timeNow = DateTime.Now;
        return _applicationDbContext.CustomerCoupon.FirstOrDefault(x =>
            x.StartTimeUse <= timeNow && x.EndTimeUse >= timeNow && x.Status == 0 &&
            x.Flag == 0 && x.CustomerId == customerId && x.Code == couponCode);
    }

    public CustomerCoupon FindCouponInActiveByCustomerId(string couponCode, int? ordersCustomerId)
    {
        return _applicationDbContext.CustomerCoupon.FirstOrDefault(x =>
             x.Status == 1 && x.Flag == 0 && x.CustomerId == ordersCustomerId && x.Code == couponCode);
    }

    public List<CustomerCoupon> GetALlByCustomerId(int? customerId)
    {
        var timeNow = DateTime.Now;
        return _applicationDbContext.CustomerCoupon.Where(x =>
            x.StartTimeUse <= timeNow && 
            x.EndTimeUse >= timeNow &&
            x.Status == 0 &&
            x.Flag == 0 &&
            x.CustomerId == customerId
        ).ToList();
    }
    public CustomerCoupon FindCouponActiveEdit(int customerId, string couponCode, string couponCodeOld)
    {
        DateTime timeNow = DateTime.Now;
        return _applicationDbContext.CustomerCoupon.FirstOrDefault(x =>
            x.StartTimeUse <= timeNow && x.EndTimeUse >= timeNow && (x.Status == 0 || x.Code == couponCodeOld ) &&
            x.Flag == 0 && x.CustomerId == customerId && x.Code == couponCode);
    }

    public List<CustomerCoupon> GetALlByCustomerIdOrderId(int? customerId, string voucherAction)
    {
        DateTime timeNow = DateTime.Now;
        return _applicationDbContext.CustomerCoupon.Where(x =>
            x.StartTimeUse <= timeNow && 
            x.EndTimeUse >= timeNow &&
            (x.Status == 0  ||  x.Code == voucherAction)&&
            x.Flag == 0 &&
            x.CustomerId == customerId
        ).ToList();
    }

    public IQueryable<CustomerCoupon> GetAllNoDateByCustomerId(int customerId)
    {
        var rs = _applicationDbContext.CustomerCoupon.Where(x => x.Flag == 0 && x.CustomerId == customerId)
            .Include(x => x.Customer);
        return rs;
    }
}
