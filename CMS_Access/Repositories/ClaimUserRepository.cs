using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CMS_EF.DbContext;
using CMS_Lib.DI;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories
{
    public interface IClaimUserRepository : IScoped
    {
        List<string> GetControllerActionRoleByUserClaimType(int userId, string claimType);

        List<MenuNav> GetMenuByUserAndControllerAction(List<string> listControllerAction);
    }

    public class ClaimUserRepository : IClaimUserRepository, IScoped
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ClaimUserRepository(ApplicationDbContext applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
        }


        public List<string> GetControllerActionRoleByUserClaimType(int userId, string claimType)
        {
            var listControllerAction = (from a in _applicationDbContext.ApplicationActions.AsNoTracking()
                                        join rc in _applicationDbContext.RoleClaims.AsNoTracking() on a.Id.ToString() equals rc.ClaimValue
                                        join r in _applicationDbContext.Roles.AsNoTracking() on rc.RoleId equals r.Id
                                        join ur in _applicationDbContext.UserRoles.AsNoTracking() on r.Id equals ur.RoleId
                                        where ur.UserId == userId && rc.ClaimType == claimType && a.Flag == 0
                                        select a.Name).Distinct().ToList();
            return listControllerAction;
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: System.String")]
        public List<MenuNav> GetMenuByUserAndControllerAction(List<string> listControllerAction)
        {

            var rs = (from m in _applicationDbContext.Menu.AsNoTracking()
                      join a in _applicationDbContext.ApplicationActions on new {Id = m.ActionId.Value, Flag = 0} equals new {Id = a.Id, Flag = a.Flag} into g1
                      from g in g1.DefaultIfEmpty()
                      where m.Status == 1 && m.Flag == 0 &&
                            (m.ActionId == 0 || listControllerAction.Any(c => c == g.Name))
                      select new MenuNav()
                      {
                          Id = m.Id,
                          Name = m.Name,
                          CssClass = m.CssClass,
                          Pid = m.Pid,
                          Url = m.Url,
                          Lft = m.Lft ?? 0,
                          Lvl = m.Lvl ?? 0,
                          Rgt = m.Rgt ?? "",
                          ActionName = g.Name,
                          ControllerName = g.Controller.Name
                      }).OrderBy(x => x.Lft).ToList();
            return rs;
        }
    }
}
