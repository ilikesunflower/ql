using CMS_EF.DbContext;
using CMS_EF.Models.Articles;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Categories;

public interface IArticleRepository : IBaseRepository<Article>, IScoped
{
    
}

public class ArticleRepository : BaseRepository<Article>, IArticleRepository
{
    public ArticleRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
    {

    }
}