using CMS_Lib.DI;
using CMS_WareHouse.KiotViet;
using CMS_WareHouse.KiotViet.Models;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Products.Services;

public interface IProductService : IScoped
{
    ProductDetail GetWareHouseByCode(string code);
}

public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _iLogger;
    private readonly IKiotVietService _iKiotVietService;
    
    public ProductService(ILogger<ProductService> iLogger, IKiotVietService iKiotVietService)
    {
        _iLogger = iLogger;
        _iKiotVietService = iKiotVietService;
    }

    public ProductDetail GetWareHouseByCode(string code)
    {
        if (!string.IsNullOrEmpty(code))
        {
           var rs = this._iKiotVietService.FindProductDetail(code);
           return rs;
        }

        return null;
    }
}