using CMS_EF.DbContext;
using CMS_EF.Models.Identity;
using CMS_Lib.DI;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS_Access.Repositories
{
    public interface IApplicationControllerRepository : IBaseRepository<ApplicationController>, IScoped
    {
        IEnumerable<ApplicationController> GetAllController();

        ApplicationController GetControllerByName(string name);
        ApplicationController GetControllerById(int id);

        bool Delete(int? id);

        List<object> GetControllerActionAll();
    }
    public class ApplicationControllerRepository : BaseRepository<ApplicationController>, IApplicationControllerRepository
    {
        private readonly IConfigurationSection _claimType;
        public ApplicationControllerRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context, IConfiguration configuration) : base(applicationDbContext, context)
        {
            this._claimType = configuration.GetSection(CmsClaimType.ClaimType);
        }

        public override bool IsCheckById(int id)
        {
            return ApplicationDbContext.ApplicationControllers.Any(x => x.Flag == 0 && x.Id == id);
        }

        public override ApplicationController FindById(int id)
        {
            var result = ApplicationDbContext.ApplicationControllers.Include(x => x.ApplicationActions.Where(c => c.Flag == 0)).FirstOrDefault(x => x.Flag == 0 && x.Id == id);
            return result;
        }
        public IEnumerable<ApplicationController> GetAllController()
        {
            var result = ApplicationDbContext.ApplicationControllers.Include(x => x.ApplicationActions.Where(c => c.Flag == 0)).Where(x => x.Flag == 0);
            return result;
        }

        public ApplicationController GetControllerByName(string name)
        {
            var result = ApplicationDbContext.ApplicationControllers.Include(x => x.ApplicationActions.Where(c => c.Flag == 0)).FirstOrDefault(x => x.Flag == 0 && x.Name == name);
            return result;
        }

        public ApplicationController GetControllerById(int id)
        {
            var result = ApplicationDbContext.ApplicationControllers.Include(x => x.ApplicationActions.Where(c => c.Flag == 0)).FirstOrDefault(x => x.Flag == 0 && x.Id == id);
            return result;
        }


        public bool Delete(int? id)
        {
            using (IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction())
            {
                try
                {
                    var controllers = ApplicationDbContext.ApplicationControllers.FirstOrDefault(x => x.Flag == 0 && x.Id == id);
                    if (controllers != null)
                    {
                        var actions = ApplicationDbContext.ApplicationActions.Where(x => x.Flag == 0 && x.Controller.Id == controllers.Id);
                        var actionString = actions.Select(x => x.Id.ToString()).ToList();
                        if (actionString.Count > 0)
                        {
                            // var roleClaimn = applicationDbContext.RoleClaims.Where(x => x.ClaimType == _claimType.GetValue<String>(CMSClaimType.ControllerAction) && actionString.Contains(x.ClaimValue));
                            var roleClaimn = ApplicationDbContext.RoleClaims.Where(x => x.ClaimType == _claimType[CmsClaimType.ControllerAction] && actionString.Contains(x.ClaimValue));
                            ApplicationDbContext.RoleClaims.RemoveRange(roleClaimn);
                            ApplicationDbContext.SaveChanges();
                        }
                        ApplicationDbContext.ApplicationActions.RemoveRange(actions);
                        ApplicationDbContext.SaveChanges();
                        ApplicationDbContext.ApplicationControllers.Remove(controllers);
                        ApplicationDbContext.SaveChanges();
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public List<object> GetControllerActionAll()
        {
            var result = ApplicationDbContext.ApplicationControllers.Include(x => x.ApplicationActions.Where(c => c.Flag == 0)).Where(x => x.Flag == 0);
            return new List<object> { result.ToList() };
        }

    }
}
