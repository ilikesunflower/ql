using System;
using System.Collections.Generic;
using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories.Customers;

public interface ICustomerTrackingRepository : IBaseRepository<CustomerTracking>, IScoped
{
    List<NumberOfCustomerGroups> GetNumberOfCustomerGroups(string txtSearch, DateTime startDate, DateTime endDate,
        int? type);

    IQueryable<TrackingOfCustomer> GetTypeCustomerActiveDetails(string txtSearch, DateTime startDate, DateTime endDate,
        int? type);
}

public class CustomerTrackingRepository : BaseRepository<CustomerTracking>, ICustomerTrackingRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public CustomerTrackingRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

    public List<NumberOfCustomerGroups> GetNumberOfCustomerGroups(string txtSearch, DateTime startDate, DateTime endDate, int? type)
    {
        var  customerQuery = _applicationDbContext.Customer.AsQueryable();
        if (!string.IsNullOrEmpty(txtSearch))
        {
            customerQuery = customerQuery.Where(x => EF.Functions.Like(x.UserName, "%" + txtSearch.Trim() + "%"));
        }

        if (type != null)
        {
            customerQuery = customerQuery.Where(x => x.TypeGroup == type);
        }

        var query = customerQuery.Include(x => x.CustomerTracking)
            .Where(x => x.CustomerTracking.Any(customerTracking => customerTracking.ActiveTimeStart >= startDate && customerTracking.ActiveTimeEnd <= endDate))
            .GroupBy(x => x.TypeGroup)
            .Select(x => new NumberOfCustomerGroups
            {
                Type = x.Key,
                Count = x.Count()
            });
          
           return query.ToList();
    }

    public IQueryable<TrackingOfCustomer> GetTypeCustomerActiveDetails(string txtSearch, DateTime startDate, DateTime endDate,
        int? type)
    {
        var customersQueryable = _applicationDbContext.Customer.Where(x => x.Flag == 0);
        if (!string.IsNullOrEmpty(txtSearch))
        {
            customersQueryable =
                customersQueryable.Where(x => EF.Functions.Like(x.UserName, "%" + txtSearch.Trim() + "%"));
        }

        if (type != null)
        {
            customersQueryable = customersQueryable.Where(x => x.TypeGroup == type);
        }

        return _applicationDbContext
            .CustomerTracking
            .Where(x => x.ActiveTimeStart >= startDate && x.ActiveTimeEnd <= endDate)
            .Join(customersQueryable,
                tracking => tracking.CustomerId,
                customer => customer.Id,
                (tracking, customer) => new TrackingOfCustomer
                {
                    Id = customer.Id,
                    Org = customer.Org,
                    TypeGroup = customer.TypeGroup,
                    Username = customer.UserName,
                    FullName = customer.FullName,
                    ActiveTime = tracking.ActiveTimeStart
                });
    }
}

public class NumberOfCustomerGroups
{
    public int? Type { set; get; }
    public double Count { set; get; }
}

public class TrackingOfCustomer
{
    public int Id { set; get; }
    public string FullName { set; get; }
    public string Username { set; get; }
    public string Org { set; get; }
    public int? TypeGroup { set; get; }
    public DateTime? ActiveTime { set; get; }
}