using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using CMS_EF.DbContext;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Customers;

public interface IHistoryFileChargePointRepository: IBaseRepository<HistoryFileChargePoint>, IScoped
{
    bool IsCodeAlreadyExist(string code);
    List<HistoryFileChargePoint> FindByIds(List<int> ids);
}

public class HistoryFileChargePointRepository: BaseRepository<HistoryFileChargePoint>, IHistoryFileChargePointRepository
{
    public HistoryFileChargePointRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
    }

    public bool IsCodeAlreadyExist(string code)
    {
        return ApplicationDbContext.HistoryFileChargePoint.Any(x => x.Flag == 0 && x.Code == code);
    }

    public List<HistoryFileChargePoint> FindByIds(List<int> ids)
    {
        return ApplicationDbContext.HistoryFileChargePoint
            .Where(x => x.Flag == 0 && ids.Contains(x.Id))
            .ToList();
    }
}