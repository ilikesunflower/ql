using System;
using System.Linq;
using CMS_Access.Repositories.Customers;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;

namespace CMS_App_Api.Services.Customers;

public interface ICustomerCouponServices:IScoped
{
    CustomerCoupon FindCouponActiveByCustomerId(int customerId,string couponCode);
    CustomerCoupon FindCouponActiveEdit(int customerId,string couponCode, string couponCodeOld);
    CustomerCoupon FindCouponById(int id);
    void Change(CustomerCoupon customerPoint);
    void Use(CustomerCoupon customerCoupon, CMS_EF.Models.Orders.Orders orderCreated);

    void Revert(CMS_EF.Models.Orders.Orders orders);

}

public class CustomerCouponServices : ICustomerCouponServices
{
    private readonly ICustomerCouponRepository _customerCouponRepository;
    private readonly ICustomerCouponLogRepository _customerCouponLogRepository;

    public CustomerCouponServices(ICustomerCouponRepository customerCouponRepository, ICustomerCouponLogRepository customerCouponLogRepository)
    {
        _customerCouponRepository = customerCouponRepository;
        _customerCouponLogRepository = customerCouponLogRepository;
    }
    public CustomerCoupon FindCouponById(int id)
    {
        return _customerCouponRepository.FindById(id);
    }
    public CustomerCoupon FindCouponActiveByCustomerId(int customerId, string couponCode)
    {
        return _customerCouponRepository.FindCouponActiveByCustomerId(customerId, couponCode);
    }
    public CustomerCoupon FindCouponActiveEdit(int customerId, string couponCode, string couponCodeOld)
    {
        return _customerCouponRepository.FindCouponActiveEdit(customerId, couponCode, couponCodeOld);

    }
    public void Change(CustomerCoupon customerPoint)
    {
        _customerCouponRepository.Update(customerPoint);
    }

    public void Use(CustomerCoupon customerCoupon, CMS_EF.Models.Orders.Orders orderCreated)
    {
        customerCoupon.LastModifiedAt = DateTime.Now;
        customerCoupon.Status = 1;
        _customerCouponRepository.Update(customerCoupon);
    }

    public void Revert(CMS_EF.Models.Orders.Orders orders)
    {
        var customerCoupon = _customerCouponRepository.FindCouponInActiveByCustomerId(orders.CouponCode, orders.CustomerId);
        if (customerCoupon == null)
        {
            return;
        }
        customerCoupon.LastModifiedAt = DateTime.Now;
        customerCoupon.Status = 0;
        _customerCouponRepository.Update(customerCoupon);
    }
}