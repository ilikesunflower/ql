using System.Linq;
using System.Linq.Dynamic.Core;
using CMS_EF.DbContext;
using CMS_EF.Models.Ship;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Ship;

public interface IShipGhnRepository : IBaseRepository<ShipGhn>, IScoped
{
    ShipGhn FindByStatus();
}

public class ShipGhnRepository : BaseRepository<ShipGhn>, IShipGhnRepository
{
    private readonly ApplicationDbContext applicationDbContext;
    public ShipGhnRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        this.applicationDbContext = applicationDbContext;
    }

    public ShipGhn FindByStatus()
    {
        return this.applicationDbContext.ShipGhn.FirstOrDefault(x => x.Flag == 0 && x.Status == true);
    }
}