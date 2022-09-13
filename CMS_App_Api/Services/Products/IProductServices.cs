using System.Collections.Generic;
using CMS_Access.Repositories.Products;
using CMS_EF.Models.Products;
using CMS_Lib.DI;

namespace CMS_App_Api.Services.Products;

public interface IProductServices : IScoped
{
    public List<ProductSimilar> GetAllProductSimilarByIds(List<int?> ids);
    void ChangeRange(List<ProductSimilar> productSimilarsChange);
}

public class ProductServices : IProductServices
{
    private readonly IProductRepository _productRepository;
    private readonly IProductSimilarRepository _productSimilarRepository;

    public ProductServices(IProductRepository productRepository, IProductSimilarRepository productSimilarRepository)
    {
        _productRepository = productRepository;
        _productSimilarRepository = productSimilarRepository;
    }

    public List<ProductSimilar> GetAllProductSimilarByIds(List<int?> ids)
    {
        return _productSimilarRepository.GetAllByIds(ids);
    }

    public void ChangeRange(List<ProductSimilar> productSimilarsChange)
    {
        _productSimilarRepository.BulkUpdate(productSimilarsChange);
    }
}