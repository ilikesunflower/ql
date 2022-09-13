using CMS_EF.DbContext;
using CMS_EF.Models.Orders;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Orders;

public interface IOrderPartnerShipLogRepository : IBaseRepository<OrderPartnerShipLog>, IScoped
{
    
}

public class OrderPartnerShipLogRepository : BaseRepository<OrderPartnerShipLog>,IOrderPartnerShipLogRepository
{
    public OrderPartnerShipLogRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
    }
}