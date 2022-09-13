using System.Collections.Generic;
using System.Linq;
using CMS_EF.DbContext;
using CMS_EF.Models;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories
{
    public interface ILoggingRepository : IBaseRepository<Logging>, IScoped
    {
    }
    public class LoggingRepository : BaseRepository<Logging>, ILoggingRepository
    {
        public LoggingRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
        {
        }

        public override int DeleteAll(List<int> listId, bool isSoftDelete = true)
        {
            ApplicationDbContext.Logging.RemoveRange(ApplicationDbContext.Logging.Where(x => listId.Contains(x.Id)));
            ApplicationDbContext.SaveChanges();
            return base.DeleteAll(listId);
        }
    }
}
