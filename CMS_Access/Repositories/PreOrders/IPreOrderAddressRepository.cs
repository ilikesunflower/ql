
using CMS_EF.DbContext;
using CMS_EF.Models.PreOrders;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.PreOrders;

public interface IPreOrderAddressRepository : IBaseRepository<PreOrderAddress>, IScoped
{
    
}

public class PreOrderAddressRepository : BaseRepository<PreOrderAddress>, IPreOrderAddressRepository
{
    public PreOrderAddressRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
    }
}