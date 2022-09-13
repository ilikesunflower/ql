using CMS_EF.DbContext;
using CMS_EF.Models.Products;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Products;


public interface IProductPropertiesRepository : IBaseRepository<ProductProperties>, IScoped
{
    
}
public class ProductPropertiesRepository : BaseRepository<ProductProperties>, IProductPropertiesRepository
{
    public ProductPropertiesRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        
    }
    
}