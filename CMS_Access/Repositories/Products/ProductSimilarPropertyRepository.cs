using CMS_EF.DbContext;
using CMS_EF.Models.Products;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Products;

public interface IProductSimilarPropertyRepository : IBaseRepository<ProductSimilarProperty>, IScoped
{
    
}
public class ProductSimilarPropertyRepository : BaseRepository<ProductSimilarProperty>, IProductSimilarPropertyRepository
{
    public ProductSimilarPropertyRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        
    }
    
}