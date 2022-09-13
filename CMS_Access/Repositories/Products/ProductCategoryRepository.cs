using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using CMS_EF.DbContext;
using CMS_EF.Models.Products;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;

namespace CMS_Access.Repositories.Products;

public interface IProductCategoryRepository : IBaseRepository<ProductCategory>, IScoped
{
     List<ProductCategory> GetListCategory(int productId);
     IQueryable<ProductCategoryValue> GetListProductOrder(int idCategoryValue);

}
public class ProductCategoryRepository : BaseRepository<ProductCategory>, IProductCategoryRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public ProductCategoryRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

    public List<ProductCategory> GetListCategory(int productId)
    {
        var data = _applicationDbContext.ProductCategory
            .Join(_applicationDbContext.ProductCategoryProduct.Where(x => x.ProductId == productId && x.Flag == 0),
                category => category.Id,
                product => product.PcategoryId,
                (category, product) => new
                {
                    category
                }).Select(x => x.category).ToList();
        return data;
    }

    public IQueryable<ProductCategoryValue> GetListProductOrder(int idCategoryValue)
    {
        var data =  _applicationDbContext.ProductCategoryProduct.Where(x => x.PcategoryId == idCategoryValue && x.Flag == 0)
            .Join(_applicationDbContext.Products,
                category => category.ProductId,
                product => product.Id,
                (category, product) => new {category,product} )
            .Select(x => new ProductCategoryValue()
            {
                ProducCategorytProductId = x.category.Id,
                Name = x.product.Name,
                Sku = x.product.Sku,
                Image = x.product.Image,
                Ord = x.category.Ord
            });
        return data;
    }
}

public class ProductCategoryValue
{
    public int ProducCategorytProductId { get; set; }
    public string Name { get; set; }
    public string Sku { get; set; }
    public string Image { get; set; }
    public int? Ord { get; set; }
}