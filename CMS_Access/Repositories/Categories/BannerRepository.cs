using CMS_EF.DbContext;
using CMS_EF.Models.Categories;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Access.Repositories.Categories
{
    public interface IBannerRepository : IBaseRepository<Banner>, IScoped
    {

    }
    public class BannerRepository : BaseRepository<Banner> , IBannerRepository
    {
        public BannerRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
        {

        }
        
    }
}
