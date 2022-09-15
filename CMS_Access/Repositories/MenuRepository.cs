using CMS_EF.DbContext;
using CMS_EF.Models;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS_Access.Repositories
{
    public interface IMenuRepository : IBaseRepository<Menu>, IScoped
    {
        List<Menu> GetDisplayMenu();

        Menu FindByName(string name);

        List<MenuNav> GetMenuNav();

        bool Delete(int id);

        bool ChangeStatus(int id, int status);

        Menu InsertMenu(Menu menu);

        bool UpdateMenu(Menu menu);

        void UpdateOrder(List<int> ids, int parentId);
        void UpdateChildrenOrder(Menu parent);
        
    }

    public class MenuRepository : BaseRepository<Menu>, IMenuRepository
    {
        public MenuRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
        {
            this.Context = context;
        }

        public override IQueryable<Menu> FindAll()
        {
            return ApplicationDbContext.Menu.AsNoTracking().Where(x => x.Flag == 0 && x.Pid != 0);
        }

        public List<Menu> GetDisplayMenu()
        {
            return ApplicationDbContext.Menu.AsNoTracking().Where(x => x.Flag == 0 && x.Pid != 0).OrderBy(x => x.Rgt).ToList();
        }

        public Menu FindByName(string name)
        {
            return ApplicationDbContext.Menu.FirstOrDefault(x => x.Flag == 0 && x.Name == name);
        }

        public List<MenuNav> GetMenuNav()
        {
            var action = (from ur in ApplicationDbContext.UserRoles
                          join rc in ApplicationDbContext.RoleClaims on ur.RoleId equals rc.RoleId
                          where ur.UserId == UserId
                          select new
                          {
                              rc.ClaimValue
                          });
            var rs = (from m in ApplicationDbContext.Menu
                      join a in ApplicationDbContext.ApplicationActions on m.ActionId equals a.Id into g1
                      where m.Status == 1 && m.Flag == 0 &&
                            (m.ActionId == 0 || action.Any(c => c.ClaimValue == m.ActionId.ToString()))
                      from g in g1.DefaultIfEmpty()
                      select new MenuNav()
                      {
                          Id = m.Id,
                          Name = m.Name,
                          CssClass = m.CssClass,
                          Pid = m.Pid,
                          Url = m.Url,
                          ControllerId = m.ControllerId ?? 0,
                          ActionId = m.ActionId ?? 0,
                          Lft = m.Lft ?? 0,
                          Lvl = m.Lvl ?? 0,
                          Rgt = m.Rgt ?? "",
                          ActionName = g.Name,
                          ControllerName = g.Controller.Name
                      }).OrderBy(x => x.Lvl);
            return rs.ToList();
        }

        public bool Delete(int id)
        {
            DateTime t = DateTime.Now;
            var menu = ApplicationDbContext.Menu.Find(id);
            menu.Flag = -1;
            menu.LastModifiedAt = t;
            menu.LastModifiedBy = UserId;
            var menuChid = ApplicationDbContext.Menu.Where(x =>  x.Rgt.StartsWith(menu.Rgt)).ToList();
            menuChid.ForEach(x =>
            {
                x.Flag = -1;
                x.LastModifiedAt = t;
                x.LastModifiedBy = UserId;
            });
            ApplicationDbContext.Menu.UpdateRange(menuChid);
            ApplicationDbContext.Menu.Update(menu);
            ApplicationDbContext.SaveChanges();
            return true;
        }

        [Obsolete]
        public Menu InsertMenu(Menu menu)
        {
            using IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                Menu pMenu = ApplicationDbContext.Menu.FirstOrDefault(x => x.Id == menu.Pid);
                if (pMenu != null)
                {
                    int index =  ApplicationDbContext.Menu.Count(w => w.Pid == menu.Pid && w.Flag == 0);
                    String pRgt = pMenu.Rgt+index ;
                    Menu insertMenu = menu;
                    insertMenu.Lft = ApplicationDbContext.Menu.Where(w => w.Pid == menu.Pid && w.Flag == 0).Max(x => x.Lft) + 1;
                    insertMenu.Rgt = pRgt;
                    insertMenu.Lvl = pMenu.Lvl + 1;
                    insertMenu.CreatedAt = DateTime.Now;
                    insertMenu.LastModifiedAt = DateTime.Now;
                    insertMenu.CreatedBy = UserId;
                    insertMenu.LastModifiedBy = UserId;
                    ApplicationDbContext.Menu.Add(insertMenu);
                    ApplicationDbContext.SaveChanges();
                    transaction.Commit();
                    return insertMenu;
                }
                return null;
            }
            catch
            {
                transaction.Rollback();
                return null;
            }
        }

        [Obsolete]
        public bool UpdateMenu(Menu menu)
        {
            using IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                var updateMenu =  ApplicationDbContext.Menu.FirstOrDefault(x => x.Id == menu.Id && x.Flag == 0);
                if (updateMenu != null)
                {
                    updateMenu.Name = menu.Name;
                    updateMenu.CssClass = menu.CssClass;
                    updateMenu.Url = menu.Url;
                    updateMenu.ControllerId = menu.ControllerId;
                    updateMenu.ActionId = menu.ActionId;
                    updateMenu.Status = menu.Status;
                    updateMenu.LastModifiedAt = DateTime.Now;
                    updateMenu.LastModifiedBy = UserId;
                    if (updateMenu.Pid != menu.Pid)
                    {
                        Menu parent = ApplicationDbContext.Menu.Find(menu.Pid );
                        if (parent == null)
                        {
                            return false;
                        }
                        updateMenu.Rgt = parent.Rgt + 0;
                        updateMenu.Lvl = parent.Lvl - 1;
                        updateMenu.Pid = menu.Pid;
                        UpdateChildrenOrder(updateMenu);
                        ApplicationDbContext.Menu.Update(updateMenu);
                        ApplicationDbContext.SaveChanges();
                    }
                    else
                    {
                        ApplicationDbContext.Menu.Update(updateMenu);
                        ApplicationDbContext.SaveChanges();
                    }
                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
            transaction.Rollback();
            return false;
        }

        public void UpdateOrder(List<int> ids, int parentId)
        {
            using IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction();
            try
            {
                Menu parent = ApplicationDbContext.Menu.Find(parentId);
                if (parent == null)
                {
                    return;
                }
                List<Menu> menus = ApplicationDbContext.Menu.Where(menu => ids.Contains(menu.Id)).ToList();
                if (menus.Count < 1 )
                {
                    return;
                }
                List<Menu> updateMenus = new List<Menu>();
                menus.ForEach(menu =>
                {
                    int index = ids.FindIndex(id => id == menu.Id) + 1;
                    String rgt = parent.Rgt + index;
                    menu.Lvl = parent.Lvl+1;
                    menu.Pid = parentId;
                    menu.Lft = parent.Lft + index;
                    menu.Rgt = rgt;
                    UpdateChildrenOrder(menu);
                    updateMenus.Add(menu);
                });
                
                ApplicationDbContext.Menu.UpdateRange(updateMenus);
                ApplicationDbContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public void UpdateChildrenOrder(Menu parent)
        {
            List<Menu> childrenMenus = ApplicationDbContext.Menu.Where(x=> x.Pid == parent.Id).OrderBy(x => x.Lft).ToList();
            if (childrenMenus.Count < 1)
            {
                return;
            }
            int index = 1;
            childrenMenus.ForEach(menu =>
            {
                menu.Lvl = parent.Lvl + 1;
                menu.Lft = index;
                menu.Rgt = parent.Rgt + index;
                UpdateChildrenOrder(menu);
                index++;
            });
            ApplicationDbContext.Menu.UpdateRange(childrenMenus);
            ApplicationDbContext.SaveChanges();
        }

        public bool ChangeStatus(int id, int status)
        {
            var menu = ApplicationDbContext.Menu.Find(id);
            DateTime t = DateTime.Now;
            menu.Status = status;
            menu.LastModifiedAt = t;
            menu.LastModifiedBy = UserId;
            if (status == 0)
            {
                var menuChid = ApplicationDbContext.Menu.Where(x => x.Pid == menu.Id).ToList();
                menuChid.ForEach(x =>
                {
                    x.Status = 0;
                    x.LastModifiedAt = t;
                    x.LastModifiedBy = UserId;
                });
                ApplicationDbContext.Menu.UpdateRange(menuChid);
                ApplicationDbContext.SaveChanges();
            }

            ApplicationDbContext.Menu.Update(menu);
            ApplicationDbContext.SaveChanges();
            return true;
        }

        public override Menu FindById(int id)
        {
            return ApplicationDbContext.Menu.FirstOrDefault(x => x.Id == id && x.Flag == 0 && x.Pid != 0);
        }

        public override bool IsCheckById(int id)
        {
            return ApplicationDbContext.Menu.Any(x => x.Id == id && x.Flag == 0 && x.Pid != 0);
        }

        public override int DeleteAll(List<int> listId, bool isSoftDelete = true)
        {
            DateTime t = DateTime.Now;
            for (int i = 0; i < listId.Count; i++)
            {
                var menu = ApplicationDbContext.Menu.FirstOrDefault(x => x.Id == listId[i] && x.Flag == 0);
                if (menu != null)
                {
                    menu.Flag = -1;
                    menu.LastModifiedAt = t;
                    menu.LastModifiedBy = UserId;
                    var menuChid = ApplicationDbContext.Menu.Where(x =>  x.Rgt.StartsWith(menu.Rgt)).ToList();
                    menuChid.ForEach(x =>
                    {
                        x.Flag = -1;
                        x.LastModifiedAt = t;
                        x.LastModifiedBy = UserId;
                    });
                    ApplicationDbContext.Menu.UpdateRange(menuChid);
                    ApplicationDbContext.Menu.Update(menu);
                    ApplicationDbContext.SaveChanges();
                }
            }
            return listId.Count;
        }
    }

    public class MenuNav
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CssClass { get; set; }

        public int Pid { get; set; }

        public string Url { get; set; }

        public int ControllerId { get; set; }
        public int ActionId { get; set; }
        public int Lft { get; set; }
        public String Rgt { get; set; }
        public int Lvl { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}
