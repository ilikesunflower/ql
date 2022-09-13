using CMS_EF.DbContext;
using CMS_EF.Models.Products;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Products;

public interface IProductPropertiesValueRepository : IBaseRepository<ProductPropertieValue>, IScoped
{
    
}
public class ProductPropertiesValueRepository : BaseRepository<ProductPropertieValue>, IProductPropertiesValueRepository
{
    public ProductPropertiesValueRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        
    }
    
}