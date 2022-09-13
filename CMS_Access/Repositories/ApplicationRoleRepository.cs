using Castle.Core.Internal;
using CMS_EF.DbContext;
using CMS_EF.Models.Identity;
using CMS_Lib.DI;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS_Access.Repositories
{
    public interface IApplicationRoleRepository : IBaseRepository<ApplicationRole>, IScoped
    {

        List<ExtendRoleController> GetControllerActionByRole(int roleId);


        ApplicationRole GetApplicationRoleByName(string name);

        IQueryable<RoleInput> GetAllRoleJoinUseRoles(int userId);


        int UpdateRoleByUser(
            int id,
            ApplicationRole editApplicationRoleView,
            List<ExtendRoleController> listRoleControllerAction
        );

        int InsertRoleByUser(
            ApplicationRole createApplicationRoleView,
            List<ExtendRoleController> listRoleControllerAction
        );

        bool DeleteApplicationRole(int id);

        ApplicationRole FindByName(string name);
    }
    public class ApplicationRoleRepository : BaseRepository<ApplicationRole>, IApplicationRoleRepository
    {
        private readonly IConfigurationSection _claimType;

        public ApplicationRoleRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context, IConfiguration configuration) : base(applicationDbContext, context)
        {
            this._claimType = configuration.GetSection(CmsClaimType.ClaimType);
        }
        public List<ExtendRoleController> GetControllerActionByRole(int roleId)
        {
            var controllerAction = ApplicationDbContext.ApplicationControllers.Where(x => x.Flag == 0).Select(x => new ExtendRoleController
            {
                Id = x.Id,
                Name = x.Name,
                Title = x.Title,
                ListAction = x.ApplicationActions.Where(a => a.Flag == 0).Select(a => new ExtendRoleAction
                {
                    Id = a.Id,
                    Title = a.Title,
                    ControllerId = a.Controller.Id,
                    Name = a.Name,
                    IsChecked = ApplicationDbContext.RoleClaims.Any(r =>
                                    r.RoleId == roleId &&
                                    r.ClaimType == _claimType[CmsClaimType.ControllerAction] &&
                                    r.ClaimValue == a.Id.ToString())
                }).ToList()
            });

            return controllerAction.ToList();
        }

        public ApplicationRole GetApplicationRoleByName(string name)
        {
            return ApplicationDbContext.Roles.FirstOrDefault(x => x.Name == name);
        }

        public IQueryable<RoleInput> GetAllRoleJoinUseRoles(int userId)
        {
            var result = (from r in ApplicationDbContext.Roles
                          join ur in ApplicationDbContext.UserRoles on
                          new { x = r.Id, x1 = userId } equals
                          new { x = ur.RoleId, x1 = ur.UserId } into g
                          from u in g.DefaultIfEmpty()
                          select new RoleInput
                          {
                              Id = r.Id,
                              Name = r.Name,
                              Description = r.Description,
                              NormalizedName = r.NormalizedName,
                              IsSelected = u != null
                          });
            return result;
        }


        public int UpdateRoleByUser(
            int id,
            ApplicationRole editApplicationRoleView,
            List<ExtendRoleController> listRoleControllerAction
        )
        {
            using (IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction())
            {
                try
                {
                    ApplicationRole role = ApplicationDbContext.Roles.FirstOrDefault(x => x.Id == id);
                    if (role != null)
                    {
                        role.Name = editApplicationRoleView.Name;
                        role.Description = editApplicationRoleView.Description;
                        //add ApplicationRoleClaim
                        if (listRoleControllerAction != null && listRoleControllerAction.Count > 0)
                        {
                            List<ApplicationRoleClaim> insertApplicationRoleClaims = new List<ApplicationRoleClaim>();
                            List<ApplicationRoleClaim> deleteApplicationRoleClaims = new List<ApplicationRoleClaim>();
                            string controllerAction = _claimType[CmsClaimType.ControllerAction];

                            foreach (var item in listRoleControllerAction)
                            {
                                if (item.ListAction != null && item.ListAction.Count > 0)
                                {
                                    foreach (var itemAction in item.ListAction)
                                    {
                                        if (itemAction.IsChecked)
                                        {
                                            if (!ApplicationDbContext.RoleClaims.Any(x => x.RoleId == role.Id && x.ClaimType == controllerAction && x.ClaimValue == itemAction.Id.ToString()))
                                            {
                                                ApplicationRoleClaim roleClaim = new ApplicationRoleClaim
                                                {
                                                    RoleId = role.Id,
                                                    ClaimType = controllerAction,
                                                    ClaimValue = itemAction.Id.ToString()
                                                };
                                                insertApplicationRoleClaims.Add(roleClaim);
                                            }
                                        }
                                        else
                                        {
                                            var roleClaimDelete = ApplicationDbContext.RoleClaims.FirstOrDefault(x => x.RoleId == role.Id && x.ClaimType == controllerAction && x.ClaimValue == itemAction.Id.ToString());
                                            if (roleClaimDelete != null)
                                            {
                                                deleteApplicationRoleClaims.Add(roleClaimDelete);
                                            }
                                        }
                                    }
                                }
                            }
                            if (insertApplicationRoleClaims.Count > 0)
                            {
                                ApplicationDbContext.RoleClaims.AddRange(insertApplicationRoleClaims);
                                // ApplicationDbContext.SaveChanges();
                            }
                            if (deleteApplicationRoleClaims.Count > 0)
                            {
                                ApplicationDbContext.RoleClaims.RemoveRange(deleteApplicationRoleClaims);
                                // ApplicationDbContext.SaveChanges();
                            }
                        }
                        ApplicationDbContext.Roles.Update(role);
                        ApplicationDbContext.SaveChanges();
                        transaction.Commit();
                        return 1;
                    }
                }
                catch (Exception)
                {

                    transaction.Rollback();

                    return -1;
                }
            }
            return -1;
        }

        public int InsertRoleByUser(
            ApplicationRole createApplicationRoleView,
            List<ExtendRoleController> listRoleControllerAction
        )
        {
            using IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                ApplicationRole role = new ApplicationRole
                {
                    Name = createApplicationRoleView.Name,
                    Description = createApplicationRoleView.Description
                };
                ApplicationDbContext.Roles.Add(role);
                ApplicationDbContext.SaveChanges();

                if (listRoleControllerAction != null && listRoleControllerAction.Count > 0)
                {
                    string controllerAction = _claimType[CmsClaimType.ControllerAction];
                    List<ApplicationRoleClaim> insertApplicationRoleClaims = new List<ApplicationRoleClaim>();
                    foreach (var item in listRoleControllerAction)
                    {
                        if (item.ListAction != null && item.ListAction.Count > 0)
                        {
                            foreach (var itemAction in item.ListAction)
                            {
                                if (itemAction.IsChecked)
                                {
                                    ApplicationRoleClaim roleClaim = new ApplicationRoleClaim
                                    {
                                        RoleId = role.Id,
                                        ClaimType = controllerAction,
                                        ClaimValue = itemAction.Id.ToString()
                                    };
                                    insertApplicationRoleClaims.Add(roleClaim);
                                }
                            }
                        }
                    }
                    if (insertApplicationRoleClaims.Count > 0)
                    {
                        ApplicationDbContext.RoleClaims.AddRange(insertApplicationRoleClaims);
                        ApplicationDbContext.SaveChanges();
                    }


                }

                transaction.Commit();
                return role.Id;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return -1;
            }
        }

        public bool DeleteApplicationRole(int id)
        {
            using IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                ApplicationRole role = ApplicationDbContext.Roles.FirstOrDefault(x => x.Id == id);
                if (role != null)
                {
                    ApplicationDbContext.RoleClaims.RemoveRange(ApplicationDbContext.RoleClaims.Where(x => x.RoleId == role.Id));
                    ApplicationDbContext.SaveChanges();
                    ApplicationDbContext.Roles.Remove(role);
                    ApplicationDbContext.SaveChanges();
                    transaction.Commit();
                    return true;
                }

            }
            catch (Exception)
            {
                transaction.Rollback();
            }

            return false;
        }

        public override int DeleteAll(List<int> listId, bool isSoftDelete = true)
        {
            using IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                for (int i = 0; i < listId.Count; i++)
                {
                    ApplicationRole role = ApplicationDbContext.Roles.FirstOrDefault(x => x.Id == listId[i]);
                    if (role != null)
                    {
                        ApplicationDbContext.RoleClaims.RemoveRange(ApplicationDbContext.RoleClaims.Where(x => x.RoleId == role.Id));
                        ApplicationDbContext.Roles.Remove(role);
                        ApplicationDbContext.SaveChanges();
                    }
                }
                transaction.Commit();
                return listId.Count;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return -1;
            }
        }

        public ApplicationRole FindByName(string name)
        {
            return ApplicationDbContext.Roles.FirstOrDefault(x => x.Name == name);
        }

        public override bool IsCheckById(int id)
        {
            return ApplicationDbContext.Roles.Any(x => x.Id == id);
        }

    }

    public class RoleInput : ApplicationRole
    {
        public bool IsSelected { get; set; }
    }
    public class ExtendRoleAction
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public int ControllerId { get; set; }
        public bool IsChecked { get; set; }
    }
    public class ExtendRoleController
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public List<ExtendRoleAction> ListAction { get; set; }

    }
}
