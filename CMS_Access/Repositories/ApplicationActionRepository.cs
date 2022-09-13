using CMS_EF.DbContext;
using CMS_EF.Models.Identity;
using CMS_Lib.DI;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace CMS_Access.Repositories
{
    public interface IApplicationActionRepository : IBaseRepository<ApplicationAction>, IScoped
    {
        IEnumerable<ApplicationAction> GetAllActions();

        void Delete(int? id);
    }
    public class ApplicationActionRepository : BaseRepository<ApplicationAction>, IApplicationActionRepository
    {
        private readonly IConfigurationSection _claimType;

        public ApplicationActionRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context, IConfiguration configuration) : base(applicationDbContext, context)
        {
            this._claimType = configuration.GetSection(CmsClaimType.ClaimType);
        }
        public IEnumerable<ApplicationAction> GetAllActions()
        {
            var result = ApplicationDbContext.ApplicationActions.Include(x => x.Controller).Where(x => x.Flag == 0 && x.Controller.Flag == 0);
            return result;
        }

        public override ApplicationAction FindById(int id)
        {
            var result = ApplicationDbContext.ApplicationActions.Include(x => x.Controller).Where(x => x.Controller.Flag == 0).FirstOrDefault(x => x.Id == id && x.Flag == 0);
            return result;
        }

        public void Delete(int? id)
        {
            var action = ApplicationDbContext.ApplicationActions.FirstOrDefault(x => x.Flag == 0 && x.Id == id);
            if (action != null)
            {
                // var roleClaimn = applicationDbContext.RoleClaims.Where(x => x.ClaimType == _claimType.GetValue<String>(CMSClaimType.ControllerAction) && x.ClaimValue == action.Id.ToString());
                var roleClaimn = ApplicationDbContext.RoleClaims.Where(x => x.ClaimType == _claimType[CmsClaimType.ControllerAction] && x.ClaimValue == action.Id.ToString());
                ApplicationDbContext.RoleClaims.RemoveRange(roleClaimn);
                ApplicationDbContext.SaveChanges();
                ApplicationDbContext.Remove(action);
                ApplicationDbContext.SaveChanges();
            }
        }
    }
}
