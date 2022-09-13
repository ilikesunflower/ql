using System.Linq;
using System.Linq.Dynamic.Core;
using CMS_EF.DbContext;
using CMS_EF.Models.WareHouse;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.WareHouse;

public interface IWhKiotVietRepository : IBaseRepository<WhKiotViet>, IScoped
{
    WhKiotViet FindByStatus();
}

public class WhKiotVietRepository : BaseRepository<WhKiotViet>, IWhKiotVietRepository
{
    private readonly ApplicationDbContext applicationDbContext;
    public WhKiotVietRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        this.applicationDbContext = applicationDbContext;
    }

    public WhKiotViet FindByStatus()
    {
        return this.applicationDbContext.WhKiotViet.FirstOrDefault(x => x.Flag == 0 && x.Status == 1);
    }
}

