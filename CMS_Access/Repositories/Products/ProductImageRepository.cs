using CMS_EF.DbContext;
using CMS_EF.Models.Products;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Products;

public interface IProductImageRepository : IBaseRepository<ProductImage>, IScoped
{
    
}
public class ProductImageRepository : BaseRepository<ProductImage>, IProductImageRepository
{
    public ProductImageRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        
    }
}