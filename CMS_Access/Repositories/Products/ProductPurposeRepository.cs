using CMS_EF.DbContext;
using CMS_EF.Models.Products;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Products;

public interface IProductPurposeRepository : IBaseRepository<ProductPurpose>, IScoped
{
    
}
public class ProductPurposeRepository : BaseRepository<ProductPurpose>, IProductPurposeRepository
{
    public ProductPurposeRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        
    }
}