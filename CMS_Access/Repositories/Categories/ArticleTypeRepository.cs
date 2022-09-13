using CMS_EF.DbContext;
using CMS_EF.Models.Articles;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Access.Repositories.Categories
{
    public interface IArticleTypeRepository : IBaseRepository<ArticleType>, IScoped
    {

    }
    public class ArticleTypeRepository : BaseRepository<ArticleType>, IArticleTypeRepository
    {
        public ArticleTypeRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
        {

        }
    }
}
