using CMS_EF.DbContext;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Customers;

public interface IFeNotificationRepository : IBaseRepository<FeNotification>, IScoped
{
    
}

public class FeNotificationRepository : BaseRepository<FeNotification>, IFeNotificationRepository
{
    public FeNotificationRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
    }
}