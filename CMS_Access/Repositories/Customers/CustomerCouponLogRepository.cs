using System;
using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Customers;

public interface ICustomerCouponLogRepository : IBaseRepository<CustomerCouponLog>, IScoped
{
}

public class CustomerCouponLogRepository : BaseRepository<CustomerCouponLog>, ICustomerCouponLogRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public CustomerCouponLogRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

    
}