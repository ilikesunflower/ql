using System.Collections.Generic;
using System.Linq;
using CMS_Access.Repositories.Categories;
using CMS_EF.DbContext;
using CMS_EF.Models.Products;
using CMS_Lib.DI;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories.Products;


public interface IProductSimilarRepository : IBaseRepository<ProductSimilar>, IScoped
{
    List<ProductSimilar> DeleteSimilarProperTy(int productId);
    List<ProductSimilar> GetAllByIds(List<int?> ids);

    List<ProductSimilar> FindAllByProductId(int productId);

    List<ProductSimilar> FindAllByListProductId(List<int> listProductId);
}
public class ProductSimilarRepository : BaseRepository<ProductSimilar>, IProductSimilarRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public ProductSimilarRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

    public List<ProductSimilar> DeleteSimilarProperTy(int productId)
    {
        var data = _applicationDbContext.ProductSimilar.Where(x => x.ProductId == productId && x.Flag == 0)
            .Join(_applicationDbContext.ProductSimilarProperty.Where(x =>  x.Flag == 0),
                category => category.Id,
                product => product.ProductSimilarId,
                (category, product) => new
                {
                    category
                }).Select(x => x.category).ToList();
        return data;
    }

    public List<ProductSimilar> GetAllByIds(List<int?> ids)
    {
        var query = _applicationDbContext.ProductSimilar.Where(x => x.Flag == 0 && ids.Contains(x.Id));

        return query.ToList();
    }

    public List<ProductSimilar> FindAllByProductId(int productId)
    {
        return _applicationDbContext.ProductSimilar.Where(x => x.Flag == 0 && x.ProductId == productId).ToList();
    }

    public List<ProductSimilar> FindAllByListProductId(List<int> listProductId)
    {
        return _applicationDbContext.ProductSimilar.Where(x => x.Flag == 0 && listProductId.Contains(x.ProductId.Value) && !string.IsNullOrEmpty(x.Skuwh)).ToList();
    }
}