using System;
using System.Collections.Generic;
using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Customers;
using CMS_EF.Models.Orders;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Customers;

public interface ICustomerPointRepository : IBaseRepository<CustomerPoint>, IScoped
{
    List<CustomerPoint> FindByCustomerId(int customerId);
    List<CustomerPoint> FindByIds(List<int> ids);
    List<OrderPoint> FindByIdAndOrderId( int orderId);
    IQueryable<CustomerPoint> FindByFileId(int fileId);
    IQueryable<CustomerPoint> FindByFileIds(List<int> ids);
}

public class CustomerPointRepository: BaseRepository<CustomerPoint>, ICustomerPointRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public CustomerPointRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

    public List<CustomerPoint> FindByCustomerId(int customerId)
    {
        var now = DateTime.Now;
        return _applicationDbContext.CustomerPoint
            .Where(x => 
                x.Flag == 0 && 
                x.CustomerId == customerId && 
                x.StartTime <= now && 
                x.EndTime >= now
            )
            .OrderBy( x => x.EndTime )
            .ToList();
    }

    public List<CustomerPoint> FindByIds(List<int> ids)
    {
        return _applicationDbContext.CustomerPoint
            .Where(x =>  ids.Contains(x.Id))
            .ToList();
    }

    public List<OrderPoint> FindByIdAndOrderId(int orderId)
    {
        var t = DateTime.Now;
        var rs = _applicationDbContext.OrderPoint
            .Where(x => x.Flag == 0 && x.OrderId == orderId && x.StartTime <= t && x.EndTime >= t).ToList();
        return rs;
    }

    public IQueryable<CustomerPoint> FindByFileId(int fileId)
    {
       return  _applicationDbContext.CustomerPoint
           .Where(x => x.Flag == 0 && x.HistoryFileChargeFileId == fileId);
    }

    public IQueryable<CustomerPoint> FindByFileIds(List<int> ids)
    {
        return  _applicationDbContext.CustomerPoint
            .Where(x => x.Flag == 0 && ids.Contains(x.HistoryFileChargeFileId ?? 0));
    }
}