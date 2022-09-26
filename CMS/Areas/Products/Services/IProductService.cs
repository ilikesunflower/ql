using System;
using System.Collections.Generic;
using System.Linq;
using CMS.Areas.Products.Const;
using CMS.Areas.Products.Models.Product;
using CMS.Models;
using CMS.Services.Files;
using CMS_Access.Repositories.Products;
using CMS_EF.DbContext;
using CMS_EF.Models.Products;
using CMS_Lib.DI;
using CMS_Lib.Extensions.HtmlAgilityPack;
using CMS_Lib.Util;
using CMS_WareHouse.KiotViet;
using CMS_WareHouse.KiotViet.Models;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Products.Services;

public interface IProductService : IScoped
{
    ProductDetail GetWareHouseByCode(string code);
    ResultJson SaveProductEdit(EditProductModel editData, int userId);
}

public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _iLogger;
    private readonly IKiotVietService _iKiotVietService;
    private readonly IProductPurposeRepository _iProductPurposeRepository;
    private readonly IProductPropertiesRepository _iProductPropertiesRepository;
    private readonly IProductCategoryRepository _iProductCategoryRepository;
    private readonly IProductCategoryProductRepository _iProductCategoryProductRepository;
    private readonly IProductImageRepository _iProductImageRepository;
    private readonly IProductSimilarRepository _iProductSimilarRepository;
    private readonly IProductSimilarPropertyRepository _iProductSimilarPropertyRepository;
    private readonly IProductPropertiesValueRepository _iProductPropertiesValueRepository;
    private readonly IFileService _iFileService;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IProductRepository _iProductRepository;


    public ProductService(ILogger<ProductService> iLogger, IKiotVietService iKiotVietService, IFileService iFileService, IProductRepository iProductRepository,
        IProductPurposeRepository iProductPurposeRepository,
        IProductPropertiesRepository iProductPropertiesRepository,
        IProductCategoryRepository iProductCategoryRepository,
        IProductCategoryProductRepository iProductCategoryProductRepository,
       IProductImageRepository iProductImageRepository,
        ApplicationDbContext applicationDbContext,
        IProductSimilarRepository iProductSimilarRepository,
        IProductSimilarPropertyRepository iProductSimilarPropertyRepository,
        IProductPropertiesValueRepository iProductPropertiesValueRepository)
    {
        _iLogger = iLogger;
        _iKiotVietService = iKiotVietService;
        _iFileService = iFileService;
        _iProductRepository = iProductRepository;
        _iProductPurposeRepository = iProductPurposeRepository;
        _iProductPropertiesRepository = iProductPropertiesRepository;
        _iProductCategoryRepository = iProductCategoryRepository;
        _iProductCategoryProductRepository = iProductCategoryProductRepository;
        _iProductImageRepository = iProductImageRepository;
        _applicationDbContext = applicationDbContext;
        _iProductSimilarRepository = iProductSimilarRepository;
        _iProductSimilarPropertyRepository = iProductSimilarPropertyRepository;
        _iProductPropertiesValueRepository = iProductPropertiesValueRepository;
        
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

    public ResultJson SaveProductEdit(EditProductModel editData, int userId)
    {
        using var transaction = _applicationDbContext.Database.BeginTransaction();

        try
        {
        //sửa product
                     var image = _iFileService.SavingFile(editData.Image, "upload/product");
                    var product = _iProductRepository.FindById(editData.Id);
                    product.Sku = editData.Sku;
                    product.Name = editData.Name.Trim();
                    product.Weight = editData.Weight;
                    // product.Price = editData.Price;
                    product.PriceSale = editData.PriceSale;
                    product.Description = editData.Description == null
                        ? null
                        : HtmlAgilityPackService.DeleteBase64(editData.Description);
                    product.Specifications = editData.Specifications == null
                        ? null
                        : HtmlAgilityPackService.DeleteBase64(editData.Specifications);
                    product.Lead = editData.Lead == null ? null : HtmlAgilityPackService.DeleteBase64(editData.Lead);
                    product.ProductPurposeId = editData.ProductPurposeId;
                    product.Unit = editData.Unit;
                    if (editData.Image != null)
                    {
                        product.Image = image;
                    }

                    product.IsHot = editData.IsHot;
                    product.IsBestSale = editData.IsBestSale;
                    product.IsNew = editData.IsNew;
                    product.IsPromotion = editData.IsPromotion;
                    
                    product.ProductSex = editData.ProductSex;
                    product.ProductAge = editData.ProductAge;
                    int statusNotApproved = ProductCensorshipConst.NotApproved.Status;
                    int statusPending = ProductCensorshipConst.Pending.Status;
                    if (product.Org1Status == statusNotApproved)
                    {
                        product.Org1Status = statusPending;
                    }

                    if (product.Org2Status == statusNotApproved)
                    {
                        product.Org2Status = statusPending;
                    }

                    if (product.Org3Status == statusNotApproved)
                    {
                        product.Org3Status = statusPending;
                    }

                    _iProductRepository.Update(product);
            //sửa productCategory
                    if (editData.ProductCategory != null)
                    {
                        var listProductCateOld = _iProductCategoryProductRepository.FindAll()
                            .Where(x => x.ProductId == editData.Id).Select(x => x.Id).ToList();
                        _iProductCategoryProductRepository.DeleteAll(listProductCateOld);
                        foreach (var item in editData.ProductCategory)
                        {
                            var productCate = new ProductCategoryProduct()
                            {
                                ProductId = product.Id,
                                PcategoryId = item
                            };
                            _iProductCategoryProductRepository.Create(productCate);
                        }
                    }
            //Sửa imageProduct
                    if (editData.ImageList == null || editData.ImageList.Count == 0)
                    {
                        var listIOld = _iProductImageRepository.FindAll().Where(x => x.ProductId == editData.Id)
                            .ToList();
                        _iProductImageRepository.DeleteAll(listIOld);
                    }
                    else
                    {
                        var listIOld = _iProductImageRepository.FindAll().Where(x =>
                                x.ProductId == editData.Id && !editData.ImageList.Contains(x.Link))
                            .ToList();
                        _iProductImageRepository.DeleteAll(listIOld);
                    }

                    if (editData.Images != null)
                    {
                        foreach (var item in editData.Images)
                        {
                            var file = _iFileService.SavingFile(item, "upload/product");
                            var productI = new ProductImage()
                            {
                                ProductId = product.Id,
                                Link = file
                            };
                            _iProductImageRepository.Create(productI);
                        }
                    }

                //sửa productSimilar
                //Sửa properties
                    if (editData.ListName != null && editData.ListName.Count > 0)
                    {
                        if (!editData.CheckEdit)
                        {
                            var prS = _iProductSimilarRepository.FindAll().Where(x => x.ProductId == editData.Id)
                                .ToList();
                            for (int i = 0; i < editData.ListName.Count; i++)
                            {
                                var pr = prS.Where(x => x.Name == editData.ListName[i]).FirstOrDefault();
                                if (pr != null)
                                {
                                    pr.QuantityWh = editData.ListQuantity[i];
                                    pr.Price = editData.ListPrice[i];
                                    pr.Skuwh = editData.ListSkuMh[i];
                                    _iProductSimilarRepository.Update(pr);
                                }
                            }
                        }
                        else
                        {
                            var listPrs = _iProductSimilarRepository.FindAll().Where(x => x.ProductId == editData.Id)
                                .ToList();
                            var prsOld = listPrs
                                .Select(x => x.Id)
                                .ToList();
                            var prSSOld = _iProductSimilarPropertyRepository.FindAll()
                                .Where(x => prsOld.Contains(x.ProductSimilarId.Value)).Select(x => x.Id).ToList();
                            var proValue = _iProductPropertiesValueRepository.FindAll()
                                .Where(x => x.ProductId == editData.Id)
                                .Select(x => x.Id).ToList();
                            var propertise = _iProductPropertiesRepository.FindAll()
                                .Where(x => x.ProductId == editData.Id).Select(x => x.Id).ToList();

                            // _iProductSimilarRepository.DeleteAll(prsOld);
                            _iProductSimilarPropertyRepository.DeleteAll(prSSOld);
                            _iProductPropertiesValueRepository.DeleteAll(proValue);
                            _iProductPropertiesRepository.DeleteAll(propertise);
                            List<int> listP1 = new List<int>();
                            List<int> listP2 = new List<int>();
                            List<int> listP3 = new List<int>();
                            List<int> listPS = new List<int>();
                            List<ProductSimilar> listS = new List<ProductSimilar>();
                            //tạo productProperties
                            if (!string.IsNullOrEmpty(editData.Name1))
                            {
                                ProductProperties productProperties = new ProductProperties()
                                {
                                    ProductId = product.Id,
                                    Name = editData.Name1,
                                    NonName = CmsFunction.RemoveUnicode(editData.Name1),
                                    LastModifiedAt = DateTime.Now,
                                    LastModifiedBy = userId
                                };
                                _iProductPropertiesRepository.Create(productProperties);
                                foreach (var item in editData.Properties1)
                                {
                                    ProductPropertieValue productPropertieValue = new ProductPropertieValue()
                                    {
                                        ProductId = product.Id,
                                        ProductPropertiesId = productProperties.Id,
                                        Value = item,
                                        NonValue = CmsFunction.RemoveUnicode(item),
                                        LastModifiedAt = DateTime.Now,
                                        LastModifiedBy =userId
                                    };
                                    _iProductPropertiesValueRepository.Create(productPropertieValue);
                                    listP1.Add(productPropertieValue.Id);
                                }
                            }

                            if (!string.IsNullOrEmpty(editData.Name2))
                            {
                                ProductProperties productProperties = new ProductProperties()
                                {
                                    ProductId = product.Id,
                                    Name = editData.Name2,
                                    NonName = CmsFunction.RemoveUnicode(editData.Name2),
                                    LastModifiedAt = DateTime.Now,
                                    LastModifiedBy =userId
                                };
                                _iProductPropertiesRepository.Create(productProperties);
                                foreach (var item in editData.Properties2)
                                {
                                    ProductPropertieValue productPropertieValue = new ProductPropertieValue()
                                    {
                                        ProductId = product.Id,
                                        ProductPropertiesId = productProperties.Id,
                                        Value = item,
                                        NonValue = CmsFunction.RemoveUnicode(item),
                                        LastModifiedAt = DateTime.Now,
                                        LastModifiedBy = userId
                                    };
                                    _iProductPropertiesValueRepository.Create(productPropertieValue);
                                    listP2.Add(productPropertieValue.Id);
                                }
                            }

                            if (!string.IsNullOrEmpty(editData.Name3))
                            {
                                ProductProperties productProperties = new ProductProperties()
                                {
                                    ProductId = product.Id,
                                    Name = editData.Name3,
                                    NonName = CmsFunction.RemoveUnicode(editData.Name3),
                                    LastModifiedAt = DateTime.Now,
                                    LastModifiedBy = userId
                                };
                                _iProductPropertiesRepository.Create(productProperties);
                                foreach (var item in editData.Properties3)
                                {
                                    ProductPropertieValue productPropertieValue = new ProductPropertieValue()
                                    {
                                        ProductId = product.Id,
                                        ProductPropertiesId = productProperties.Id,
                                        Value = item,
                                        NonValue = CmsFunction.RemoveUnicode(item),
                                        LastModifiedAt = DateTime.Now,
                                        LastModifiedBy = userId
                                    };
                                    _iProductPropertiesValueRepository.Create(productPropertieValue);
                                    listP3.Add(productPropertieValue.Id);
                                }
                            }
                            
                            //tạo silmilar
                            for (int i = 0; i < editData.ListName.Count; i++)
                            {
                                var productS = listPrs.Where(x => x.Skuwh == editData.ListSkuMh[i]).FirstOrDefault();
                                if (productS != null)
                                {
                                    productS.Name = editData.ListName[i];
                                    productS.Price = editData.ListPrice[i];
                                    productS.QuantityWh = editData.ListQuantity[i];
                                    productS.LastModifiedAt = DateTime.Now;
                                    _iProductSimilarRepository.Update(productS);
                                    listS.Add(productS);
                                    listPS.Add(productS.Id);
                                    prsOld.Remove(productS.Id);
                                }
                                else
                                {
                                    ProductSimilar productSimilar = new ProductSimilar()
                                    {
                                        Name = editData.ListName[i],
                                        Skuwh =
                                            editData.ListSkuMh[i],
                                        QuantityWh = editData.ListQuantity[i],
                                        ProductId = product.Id,
                                        Price = editData.ListPrice[i],
                                        LastModifiedAt = DateTime.Now
                                    };
                                    _iProductSimilarRepository.Create(productSimilar);
                                    listS.Add(productSimilar);
                                    listPS.Add(productSimilar.Id);
                                }
                               
                            }
                            _iProductSimilarRepository.DeleteAll(prsOld);

                            List<List<int>> lisTT = new List<List<int>>();

                            foreach (var it1 in listP1)
                            {
                                if (listP2.Count == 0)
                                {
                                    var check1 = new List<int>() {it1};
                                    lisTT.Add(check1);
                                }

                                foreach (var it2 in listP2)
                                {
                                    if (listP3.Count == 0)
                                    {
                                        var check1 = new List<int>() {it1, it2};
                                        lisTT.Add(check1);
                                    }

                                    foreach (var it3 in listP3)
                                    {
                                        var check1 = new List<int>() {it1, it2, it3};
                                        lisTT.Add(check1);
                                    }
                                }
                            }

                            if (listPS.Count != lisTT.Count)
                            {
                                return new OutputObject(400, new{}, "lỗi").Show();

                            }

                            for (int i = 0; i < lisTT.Count; i++)
                            {
                                foreach (var item in lisTT[i])
                                {
                                    ProductSimilarProperty productSimilarProperty = new ProductSimilarProperty()
                                    {
                                        ProductSimilarId = listPS[i],
                                        ProductPropertiesValueId = item,
                                        LastModifiedAt = DateTime.Now,
                                        LastModifiedBy = userId
                                    };
                                    _iProductSimilarPropertyRepository.Create(productSimilarProperty);
                                }

                                ProductSimilar val = listS[i];
                                string stringIdProperties = string.Join(",", lisTT[i]);
                                val.ProductPropertiesValue = stringIdProperties;
                                _iProductSimilarRepository.Update(val);
                            }
                        }
                    }
                    else
                    {
                        var prs = _iProductSimilarRepository.FindAll().Where(x => x.ProductId == editData.Id).ToList();
                        var prsOld = prs.Select(x => x.Id)
                            .ToList();
                        var prSSOld = _iProductSimilarPropertyRepository.FindAll()
                            .Where(x => prsOld.Contains(x.ProductSimilarId.Value)).Select(x => x.Id).ToList();
                        if (prsOld.Count == 1 && prSSOld.Count == 0)
                        {
                            var pr = prs.FirstOrDefault();
                            pr.Skuwh = editData.CodeStock;
                            pr.QuantityWh = editData.QuantityStock;
                            pr.Price = editData.Price;
                            pr.LastModifiedAt = DateTime.Now;
                            _iProductSimilarRepository.Update(pr);
                        }
                        else
                        {
                            var proValue = _iProductPropertiesValueRepository.FindAll()
                                .Where(x => x.ProductId == editData.Id)
                                .Select(x => x.Id).ToList();
                            var propertise = _iProductPropertiesRepository.FindAll()
                                .Where(x => x.ProductId == editData.Id).Select(x => x.Id).ToList();

                            _iProductSimilarPropertyRepository.DeleteAll(prSSOld);
                            _iProductPropertiesValueRepository.DeleteAll(proValue);
                            _iProductPropertiesRepository.DeleteAll(propertise);
                            var prS = prs.Where(x =>  x.Skuwh == editData.CodeStock)
                                .FirstOrDefault();
                            if (prS != null)
                            {
                                prS.Skuwh = editData.CodeStock;
                                prS.QuantityWh = editData.QuantityStock;
                                prS.Price = editData.Price;
                                _iProductSimilarRepository.Update(prS);
                                prsOld.Remove(prS.Id);
                            }
                            else
                            {
                                ProductSimilar proS = new ProductSimilar()
                                {
                                    Skuwh = editData.CodeStock,
                                    QuantityWh = editData.QuantityStock,
                                    ProductId = product.Id,
                                    LastModifiedAt = DateTime.Now,
                                    Price = editData.Price
                                };
                                _iProductSimilarRepository.Create(proS);
                            }
                            _iProductSimilarRepository.DeleteAll(prsOld);

                        }
                     
                    }
                transaction.Commit();
                return  new OutputObject(200, new{}, "").Show();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return new OutputObject(400, new{}, e.Message,e.Message).Show();
        }
         
    }

}