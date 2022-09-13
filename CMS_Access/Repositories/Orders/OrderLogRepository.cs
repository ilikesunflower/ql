using CMS_EF.DbContext;
using CMS_EF.Models.Orders;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Orders;

public interface IOrderLogRepository : IBaseRepository<OrderLog>, IScoped
{
    
}

public class OrderLogRepository : BaseRepository<OrderLog>,IOrderLogRepository
{
    public OrderLogRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
    }
}