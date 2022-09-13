

using System.Linq;
using System.Linq.Dynamic.Core;
using CMS_EF.DbContext;
using CMS_EF.Models.PreOrders;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories.PreOrders;

public interface IPreOrderRepository : IBaseRepository<PreOrder>, IScoped
{
    IQueryable<PreOrder> FindAllInfo();
    PreOrder FindAllInfo(int id);
}

public class PreOrderRepository : BaseRepository<PreOrder>, IPreOrderRepository
{
    public PreOrderRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
    }

    public IQueryable<PreOrder> FindAllInfo()
    {
        return ApplicationDbContext.PreOrder
            .Include(x => x.Product)
            .Include(x => x.ProductSimilar)
            .ThenInclude(x => x.ProductSimilarProperty)
            .ThenInclude(x => x.ProductPropertiesValue)
            .ThenInclude(x => x.ProductProperties)
            .Include(x => x.Customer)
            .Include(x => x.PreOrderAddress).ThenInclude(x => x.Province)
            .Include(x => x.PreOrderAddress).ThenInclude(x => x.District)
            .Include(x => x.PreOrderAddress).ThenInclude(x => x.Commune)
            .Where(x => x.Flag == 0);
    }

    public PreOrder FindAllInfo(int id)
    {
        return FindAllInfo().FirstOrDefault(x => x.Id == id);
    }
}