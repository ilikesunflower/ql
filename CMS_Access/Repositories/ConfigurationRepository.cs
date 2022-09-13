using CMS_EF.DbContext;
using CMS_EF.Models;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS_Access.Repositories
{
    public interface IConfigurationRepository : IBaseRepository<Configuration>, IScoped
    {
        Configuration FindByName(string name);
        Configuration FindByVal(string name);
    }
    public class ConfigurationRepository : BaseRepository<Configuration>, IConfigurationRepository
    {
        public ConfigurationRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
        {

        }

        public override IQueryable<Configuration> FindAll()
        {
            return ApplicationDbContext.Configuration.Where(x => x.Flag == 0).AsNoTracking();
        }

        public override void Delete(Configuration entity, bool isSoftDelete = true)
        {
            entity.Flag = -1;
            entity.LastModifiedAt = DateTime.Now;
            entity.LastModifiedBy = UserId;
            ApplicationDbContext.Update(entity);
            ApplicationDbContext.SaveChanges();
        }

        public override int DeleteAll(List<int> listId, bool isSoftDelete = true)
        {
            var listData = base.ApplicationDbContext.Configuration.Where(x => x.Flag == 0 && (listId.Contains(x.Id))).ToList();
            listData.ForEach(x => x.Flag = -1);
            base.ApplicationDbContext.SaveChanges();
            return listData.Count;
        }

        public override Configuration FindById(int id)
        {
            return ApplicationDbContext.Configuration.FirstOrDefault(x => x.Flag == 0 && (x.Id == id));
        }

        public override bool IsCheckById(int id)
        {
            return ApplicationDbContext.Configuration.Any(x => x.Flag == 0 && (x.Id == id));
        }

        public Configuration FindByName(string name)
        {
            return ApplicationDbContext.Configuration.FirstOrDefault(x => x.Flag == 0 && x.Name == name);
        }

        public Configuration FindByVal(string name)
        {
            return ApplicationDbContext.Configuration.FirstOrDefault(x => x.Flag == 0 && x.Val == name);
        }
    }
}
