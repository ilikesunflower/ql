using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Numerics;
using CMS_EF.DbContext;
using CMS_EF.Models.Orders;
using CMS_Lib.DI;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Repositories.Products;


public interface IProductRepository : IBaseRepository<CMS_EF.Models.Products.Products>, IScoped
{
    List<ProductPrList> GetProductProperties(int productId);
    IQueryable<ProductIndex> GetProductAllIndex();
    CMS_EF.Models.Products.Products FindByDetailId(int id);
    List<OrderProduct> OrderProductsByIds(List<int> toList);
    List<OrderProduct> OrderProductsEditByIds(List<int> toList, int orderId);
}
public class ProductRepository : BaseRepository<CMS_EF.Models.Products.Products>, IProductRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public ProductRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(
        applicationDbContext, context)
    {
        _applicationDbContext = applicationDbContext;
    }

    public List<ProductPrList> GetProductProperties(int productId)
    {
        var rs = (from ct in _applicationDbContext.ProductProperties
            join c in _applicationDbContext.ProductPropertieValue on ct.Id equals  c.ProductPropertiesId
            where ct.Flag == 0 && c.Flag == 0 && c.ProductId == productId
            group new {c, ct} by new {ct.Id, ct.Name}
            into g
            select new ProductPrList
            {
               Name = g.Key.Name,
               ListValueName =  g.Select(x => x.c.Value).ToList()
            }).ToList();
        return rs;
    }

    public IQueryable<ProductIndex> GetProductAllIndex()
    {
        var rs = (from p in _applicationDbContext.Products
                join ps in _applicationDbContext.ProductSimilar on p.Id equals ps.ProductId
                where p.Flag == 0 && ps.Flag == 0
                group new {p, ps} by new {p.Id, p.Sku, p.Name, p.IsPublic, p.LastModifiedAt, p.Image, p.IsHot, p.IsNew, p.IsPromotion, p.IsBestSale,p.Org1Status,p.Org2Status,p.Org3Status}
                into g
                select new ProductIndex
                {
                    Name = g.Key.Name,
                    Id = g.Key.Id,
                    Sku = g.Key.Sku,
                    Org1Status = g.Key.Org1Status,
                    Org2Status = g.Key.Org2Status,
                    Org3Status = g.Key.Org3Status,
                    IsPublic = g.Key.IsPublic,
                    IsNew = g.Key.IsNew,
                    IsHot = g.Key.IsHot,
                    IsPromotion = g.Key.IsPromotion,
                    IsBestSale = g.Key.IsBestSale,
                    Image = g.Key.Image,
                    LastModifiedAt = g.Key.LastModifiedAt,
                    Quantity = g.Sum(x =>(double)(x.ps.QuantityWh ?? 0) )
                }
            );
        return rs;
    }

    public CMS_EF.Models.Products.Products FindByDetailId(int id)
    {
        var rs = _applicationDbContext.Products
            .Include(x => x.ProductImage.Where(a => a.Flag == 0))
            .Include(x => x.ProductProperties.Where(a => a.Flag == 0))
            .ThenInclude(x => x.ProductPropertieValue.Where(a => a.Flag == 0))
            .Include(x => x.ProductSimilar.Where(a => a.Flag == 0))
            .SingleOrDefault(x => x.Flag == 0 && x.Id == id);
        return rs;
    }
    public List<OrderProduct> OrderProductsByIds(List<int> productSimilarIds)
    {
        return (from products in _applicationDbContext.Products
            join productSimilar in _applicationDbContext.ProductSimilar on products.Id equals productSimilar.ProductId
            where products.Flag == 0 && productSimilar.Flag == 0 && productSimilarIds.Contains(productSimilar.Id)
            select new OrderProduct
            {
                Price =(int) productSimilar.Price,
                Quantity = productSimilar.QuantityWh,
                ProductSimilarId = productSimilar.Id,
                PriceSale = products.PriceSale,
                ProductId = products.Id,
                ProductImage = products.Image,
                ProductSimilarCodeWh = productSimilar.Skuwh,
                ProductName = products.Name,
                Weight = products.Weight,
                OrderProductSimilarProperty = ( 
                    from productSimilarProperty in _applicationDbContext.ProductSimilarProperty.Where( x => x.ProductSimilarId == productSimilar.Id )
                    join productPropertieValue in _applicationDbContext.ProductPropertieValue on productSimilarProperty.ProductPropertiesValueId equals productPropertieValue.Id
                    join productProperties in _applicationDbContext.ProductProperties on productPropertieValue.ProductPropertiesId equals productProperties.Id 
                    select new OrderProductSimilarProperty
                    {
                        Flag = 0,
                        LastModifiedAt = DateTime.Now,
                        ProductPropertiesId = productProperties.Id,
                        ProductPropertiesName = productProperties.Name,
                        ProductPropertiesValueId = productPropertieValue.Id,
                        ProductPropertiesValueName = productPropertieValue.Value,
                        ProductPropertiesValueNonName = productPropertieValue.NonValue
                    }).ToList()
            }).ToList();
    } 
    public List<OrderProduct> OrderProductsEditByIds(List<int> productSimilarIds, int orderId)
    {
        return (from products in _applicationDbContext.Products
            join productSimilar in _applicationDbContext.ProductSimilar on products.Id equals productSimilar.ProductId
            where products.Flag == 0 && productSimilar.Flag == 0 && productSimilarIds.Contains(productSimilar.Id)
            select new OrderProduct
            {
                OrderId = orderId,
                Price =(int) productSimilar.Price,
                Quantity = productSimilar.QuantityWh,
                ProductSimilarId = productSimilar.Id,
                PriceSale = products.PriceSale,
                ProductId = products.Id,
                ProductImage = products.Image,
                ProductSimilarCodeWh = productSimilar.Skuwh,
                ProductName = products.Name,
                Weight = products.Weight,
                OrderProductSimilarProperty = ( 
                    from productSimilarProperty in _applicationDbContext.ProductSimilarProperty.Where( x => x.ProductSimilarId == productSimilar.Id )
                    join productPropertieValue in _applicationDbContext.ProductPropertieValue on productSimilarProperty.ProductPropertiesValueId equals productPropertieValue.Id
                    join productProperties in _applicationDbContext.ProductProperties on productPropertieValue.ProductPropertiesId equals productProperties.Id 
                    select new OrderProductSimilarProperty
                    {
                        Flag = 0,
                        LastModifiedAt = DateTime.Now,
                        ProductPropertiesId = productProperties.Id,
                        ProductPropertiesName = productProperties.Name,
                        ProductPropertiesValueId = productPropertieValue.Id,
                        ProductPropertiesValueName = productPropertieValue.Value,
                        ProductPropertiesValueNonName = productPropertieValue.NonValue
                    }).ToList()
            }).ToList();
    }
}
public class ProductPrList
{
    public string Name { get; set; }
    public List<string> ListValueName { get; set; }
}

public class ProductIndex
{
    public int Id { get; set; }
    public string Image { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
    public double? Quantity { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsHot { get; set; }
    public bool? IsBestSale { get; set; }
    public bool? IsNew { get; set; }
    public bool? IsPromotion { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    
    
    public int? Org1Status { get; set; }
    public int? Org2Status { get; set; }
    public int? Org3Status { get; set; }
    public string Org1Comment { get; set; }
    public string Org2Comment { get; set; }
    public string Org3Comment { get; set; }

}