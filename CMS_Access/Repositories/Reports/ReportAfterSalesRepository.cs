using CMS_EF.DbContext;
using CMS_EF.Models.Reports;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Reports;
public interface IReportAfterSalesRepository : IBaseRepository<ReportAfterSales>, IScoped
{
    
}
public class ReportAfterSalesRepository : BaseRepository<ReportAfterSales>, IReportAfterSalesRepository
{
    public ReportAfterSalesRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        
    }
}