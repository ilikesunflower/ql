using CMS_EF.DbContext;
using CMS_EF.Models;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS_Access.Repositories
{
    public interface IFilesRepository : IBaseRepository<Files>, IScoped
    {
        Task<int> CountAsync();
        Boolean Delete(int id);
        List<string> GetListTypes();
        List<string> GetListCreatedAt();
    }

    public class FilesRepository : BaseRepository<Files>, IFilesRepository
    {
        public FilesRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
        {

        }

        public override IQueryable<Files> FindAll()
        {
            return base.FindAll().Where(x => x.Flag == 0);
        }

        public bool Delete(int id)
        {
            var rs = ApplicationDbContext.Files.FirstOrDefault(x => x.Id == id);
            if (rs != null)
            {
                rs.Flag = -1;
                ApplicationDbContext.SaveChanges();
            }

            return true;
        }
        

        public async Task<int> CountAsync()
        {
            return await ApplicationDbContext.Files.Where( file => file.Flag == 0 ).CountAsync();
        }

        public List<string> GetListTypes()
        {
            var res =  (from file in ApplicationDbContext.Files
                         select file.ContentType
                        ).Distinct();
            return res.ToList<string>();
        }
        public List<string> GetListCreatedAt()
        {
            var res = ApplicationDbContext.Files
                .OrderByDescending(b => b.CreatedAt)
                .Select(x =>  x.CreatedAt.Month + "/" + x.CreatedAt.Year)
                .Distinct()
                .ToList();

            return res;
        }
    }
}
