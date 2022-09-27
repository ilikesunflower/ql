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
using CMS.Config.Consts;
using CMS.Services.Emails;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Products.Services;

public interface IProductService : IScoped
{
    ProductDetail GetWareHouseByCode(string code);
    ResultJson SaveProductEdit(EditProductModel editData, int userId);
    ResultJson SaveProduct(CreateProductModel editData, int userId);

    
    Message ContentSendEmail(List<string> email, string title, string link);

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
                        UpdateCategory(editData.ProductCategory, editData.Id);
                    }
                //Sửa imageProduct
                updateImages(editData.ImageList, editData.Images, product.Id);
                //sửa productSimilar
                //Sửa properties
                var listPrs = _iProductSimilarRepository.FindAll().Where(x => x.ProductId == editData.Id)
                    .ToList();
                    if (editData.ListName != null && editData.ListName.Count > 0)
                    {
                        if (!editData.CheckEdit)
                        {
                        
                            for (int i = 0; i < editData.ListName.Count; i++)
                            {
                                var pr = listPrs.Where(x => x.Name == editData.ListName[i]).FirstOrDefault();
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
                          
                            var prsOld = listPrs
                                .Select(x => x.Id)
                                .ToList();
                            DeletePropertiesValue(editData.Id, prsOld);
                            List<int> listP1 = new List<int>();
                            List<int> listP2 = new List<int>();
                            List<int> listP3 = new List<int>();
                            List<int> listPS = new List<int>();
                            List<ProductSimilar> listS = new List<ProductSimilar>();
                            //tạo productProperties
                            if (!string.IsNullOrEmpty(editData.Name1))
                            {
                                listP1 = CreatePropertiesValue(editData.Name1, editData.Properties1, userId,
                                    editData.Id);
                            }

                            if (!string.IsNullOrEmpty(editData.Name2))
                            {
                                listP2 = CreatePropertiesValue(editData.Name2, editData.Properties2, userId,
                                    editData.Id);
                            }

                            if (!string.IsNullOrEmpty(editData.Name3))
                            {
                                listP3 = CreatePropertiesValue(editData.Name3, editData.Properties3, userId,
                                    editData.Id);
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

                            List<List<int>> lisTT = CreateListPropertiesValue(listP1, listP2, listP3);

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
                    
                        var prsOld = listPrs.Select(x => x.Id)
                            .ToList();
                        var prSSOld = _iProductSimilarPropertyRepository.FindAll()
                            .Where(x => prsOld.Contains(x.ProductSimilarId.Value)).Select(x => x.Id).ToList();
                        if (prsOld.Count == 1 && prSSOld.Count == 0)
                        {
                            var pr = listPrs.FirstOrDefault();
                            pr.Skuwh = editData.CodeStock;
                            pr.QuantityWh = editData.QuantityStock;
                            pr.Price = editData.Price;
                            pr.LastModifiedAt = DateTime.Now;
                            _iProductSimilarRepository.Update(pr);
                        }
                        else
                        {
                            DeletePropertiesValue(editData.Id, prsOld);
                            
                            var prS = listPrs.Where(x =>  x.Skuwh == editData.CodeStock)
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

   public  ResultJson SaveProduct(CreateProductModel createData, int userId)
    {
        using var transaction = _applicationDbContext.Database.BeginTransaction();
        try
        {

                    var image = _iFileService.SavingFile(createData.Image, "upload/product");
                    int statusPending = ProductCensorshipConst.Pending.Status;
                    var product = new CMS_EF.Models.Products.Products()
                    {
                        Sku = createData.Sku,
                        Name = createData.Name.Trim(),
                        Weight = createData.Weight,
                        // Price = createData.Price,
                        PriceSale = createData.PriceSale,
                        Description = createData.Description == null
                            ? null
                            : HtmlAgilityPackService.DeleteBase64(createData.Description),
                        Lead = createData.Lead == null ? null : HtmlAgilityPackService.DeleteBase64(createData.Lead),
                        Specifications = createData.Specifications == null
                            ? null
                            : HtmlAgilityPackService.DeleteBase64(createData.Specifications),
                        ProductPurposeId = createData.ProductPurposeId,
                        Unit = createData.Unit,
                        Image = image,
                        IsHot = createData.IsHot,
                        IsNew = createData.IsNew,
                        IsBestSale = createData.IsBestSale,
                        IsPromotion = createData.IsPromotion,
                        ProductSex = createData.ProductSex,
                        ProductAge = createData.ProductAge,
                        LastModifiedAt = DateTime.Now,
                        LastModifiedBy = userId,
                        IsPublic = ProductConst.IsNotPublish,
                        Org1Status = statusPending,
                        Org2Status = statusPending,
                        Org3Status = statusPending
                    };
                    product = _iProductRepository.Create(product);
                    if (createData.ProductCategory != null)
                    {
                        foreach (var item in createData.ProductCategory)
                        {
                            var productCate = new ProductCategoryProduct()
                            {
                                ProductId = product.Id,
                                PcategoryId = item
                            };
                            _iProductCategoryProductRepository.Create(productCate);
                        }
                    }

                    if (createData.Images != null)
                    {
                        foreach (var item in createData.Images)
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

                    if (createData.ListName != null && createData.ListName.Count > 0)
                    {
                        List<int> listP1 = new List<int>();
                        List<int> listP2 = new List<int>();
                        List<int> listP3 = new List<int>();
                        List<int> listPS = new List<int>();
                        List<ProductSimilar> listS = new List<ProductSimilar>();
                        if (!string.IsNullOrEmpty(createData.Name1))
                        {
                            listP1 = CreatePropertiesValue(createData.Name1, createData.Properties1, userId,
                                product.Id);
                        
                        }

                        if (!string.IsNullOrEmpty(createData.Name2))
                        {
                            listP2 = CreatePropertiesValue(createData.Name2, createData.Properties2, userId,
                                product.Id);

                        }

                        if (!string.IsNullOrEmpty(createData.Name3))
                        {
                            listP3 = CreatePropertiesValue(createData.Name3, createData.Properties3, userId,
                                product.Id);

                        }

                        for (int i = 0; i < createData.ListName.Count; i++)
                        {
                            ProductSimilar productSimilar = new ProductSimilar()
                            {
                                Name = createData.ListName[i],
                                Skuwh = createData.ListSkuMh[i] == null
                                    ? $"{createData.Sku}{i}"
                                    : createData.ListSkuMh[i],
                                QuantityWh = createData.ListQuantity[i],
                                ProductId = product.Id,
                                Price = createData.ListPrice[i],
                                LastModifiedAt = DateTime.Now
                            };
                            _iProductSimilarRepository.Create(productSimilar);
                            listS.Add(productSimilar);
                            listPS.Add(productSimilar.Id);
                        }

                        List<List<int>> lisTT = CreateListPropertiesValue(listP1, listP2, listP3);

                        if (listPS.Count != lisTT.Count)
                        {
                            return new OutputObject(400, new{}, "err").Show();
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
                    else
                    {
                        ProductSimilar proS = new ProductSimilar()
                        {
                            Skuwh = createData.CodeStock == null ? createData.Sku :  createData.CodeStock ,
                            QuantityWh = createData.QuantityStock,
                            ProductId = product.Id,
                            LastModifiedAt = DateTime.Now,
                            Price = createData.Price
                        };
                        _iProductSimilarRepository.Create(proS);
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
    //update images

    public void updateImages(List<string> imageList, List<IFormFile> images,  int productId  )
    {
        if (imageList == null || imageList.Count == 0)
        {
            var listIOld = _iProductImageRepository.FindAll().Where(x => x.ProductId == productId)
                .ToList();
            _iProductImageRepository.DeleteAll(listIOld);
        }
        else
        {
            var listIOld = _iProductImageRepository.FindAll().Where(x =>
                    x.ProductId == productId && !imageList.Contains(x.Link))
                .ToList();
            _iProductImageRepository.DeleteAll(listIOld);
        }

        if (images != null)
        {
            foreach (var item in images)
            {
                var file = _iFileService.SavingFile(item, "upload/product");
                var productI = new ProductImage()
                {
                    ProductId = productId,
                    Link = file
                };
                _iProductImageRepository.Create(productI);
            }
        }
    }
    //update category
    public void UpdateCategory(  List<int> productCategory, int productId)
    {
        var listProductCateOld = _iProductCategoryProductRepository.FindAll()
            .Where(x => x.ProductId == productId).Select(x => x.Id).ToList();
        _iProductCategoryProductRepository.DeleteAll(listProductCateOld);
        foreach (var item in productCategory)
        {
            var productCate = new ProductCategoryProduct()
            {
                ProductId = productId,
                PcategoryId = item
            };
            _iProductCategoryProductRepository.Create(productCate);
        }
    }
    //xoa productSimilarProperty. productPropertiesValue, productProperties

    public void DeletePropertiesValue(int productId, List<int> prsOld)
    {
        var prSSOld = _iProductSimilarPropertyRepository.FindAll()
            .Where(x => prsOld.Contains(x.ProductSimilarId.Value)).Select(x => x.Id).ToList();
        var proValue = _iProductPropertiesValueRepository.FindAll()
            .Where(x => x.ProductId == productId)
            .Select(x => x.Id).ToList();
        var propertise = _iProductPropertiesRepository.FindAll()
            .Where(x => x.ProductId == productId).Select(x => x.Id).ToList();

        // _iProductSimilarRepository.DeleteAll(prsOld);
        _iProductSimilarPropertyRepository.DeleteAll(prSSOld);
        _iProductPropertiesValueRepository.DeleteAll(proValue);
        _iProductPropertiesRepository.DeleteAll(propertise);
    }
    // create mới productPropertiesValue, productProperties
    public List<int> CreatePropertiesValue(string name, List<string> nameProperties, int userId, int productId)
    {
        List<int> rs = new List<int>();
        ProductProperties productProperties = new ProductProperties()
        {
            ProductId = productId,
            Name = name,
            NonName = CmsFunction.RemoveUnicode(name),
            LastModifiedAt = DateTime.Now,
            LastModifiedBy = userId
        };
        _iProductPropertiesRepository.Create(productProperties);
        foreach (var item in nameProperties)
        {
            ProductPropertieValue productPropertieValue = new ProductPropertieValue()
            {
                ProductId = productId,
                ProductPropertiesId = productProperties.Id,
                Value = item,
                NonValue = CmsFunction.RemoveUnicode(item),
                LastModifiedAt = DateTime.Now,
                LastModifiedBy = userId
            };
            _iProductPropertiesValueRepository.Create(productPropertieValue);
            rs.Add(productPropertieValue.Id);
        }

        return rs;
    }

    //create list propertiesvalue -> productSimilar
    public List<List<int>> CreateListPropertiesValue(List<int> listP1, List<int> listP2, List<int> listP3)
    {
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

        return lisTT;
    }
    // các trạng thái: editChange(true

    #region MyRegion

    public Message ContentSendEmail(List<string> email, string title, string link)
    {
        string content = $"<div style='font-size: 15px;'>\n" +
                         "            <div>\n" +
                         "                <span style=\"white-space:pre-wrap\">Dear Anh/Chị,</span>\n" +
                         "            </div>\n" +
                         "            <div>\n" +
                         $"                <span style=\"white-space:pre-wrap\">{AppConst.AppName} đã gửi 01 sản phẩm lên hệ thống. Vui lòng truy cập trang quản trị và thực hiện kiểm duyệt.</span>\n" +
                         "            </div>\n" +
                         "            <div>\n" +
                         $"                <span>- Đường dẫn tới trang sản phẩm cần duyệt: <a href=\"{link}\" target=\"_blank\">link tại đây</a> \n" +
                         "                </span>\n" +
                         "            </div>\n" +
                         "            <div>\n" +
                         $"                <span>- Đường dẫn trang quản trị {AppConst.AppName} <a href=\"{AppConst.Domain}\" target=\"_blank\">link tại đây</a> \n" +
                         "                </span>\n" +
                         "            </div>\n" +
                         "            <div>\n" +
                         "                <span style=\"white-space:pre-wrap\">Regards</span>\n" +
                         "            </div>\n" +
                         "        </div>";
        return new Message(email, title, content);
    }

    #endregion

}