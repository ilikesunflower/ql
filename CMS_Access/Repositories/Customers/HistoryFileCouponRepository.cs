using System.Collections.Generic;
using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models.Customers;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Customers;

public interface IHistoryFileCouponRepository : IBaseRepository<HistoryFileCoupon>, IScoped
{
     List<HistoryFileCoupon> GetListByCode(string code);
}
public class HistoryFileCouponRepository : BaseRepository<HistoryFileCoupon>, IHistoryFileCouponRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public HistoryFileCouponRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

    public List<HistoryFileCoupon> GetListByCode(string code)
    {
        var rs = _applicationDbContext.HistoryFileCoupon.Where(x => x.Flag == 0 && x.Code.ToLower() == code.ToLower())
            .ToList();
        return rs;
    }
}