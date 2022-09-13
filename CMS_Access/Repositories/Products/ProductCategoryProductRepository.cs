using CMS_EF.DbContext;
using CMS_EF.Models.Products;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Products;

public interface IProductCategoryProductRepository : IBaseRepository<ProductCategoryProduct>, IScoped
{
    
}
public class ProductCategoryProductRepository : BaseRepository<ProductCategoryProduct>, IProductCategoryProductRepository
{
    public ProductCategoryProductRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        
    }
}