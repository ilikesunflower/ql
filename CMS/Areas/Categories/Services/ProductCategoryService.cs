using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CMS_Access.Repositories.Products;
using CMS_EF.DbContext;
using CMS_EF.Models.Products;
using CMS_Lib.DI;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Categories.Services;

public interface IProductCategoryService : IScoped
{
    ProductCategory InsertProductCategory(ProductCategory productCategory);
    ProductCategory UpdateProductCategory(ProductCategory productCategory);
    void UpdateOrder(List<int> ids, int parentId);
}   
public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryRepository _iProductCategoryRepository;
    private readonly ApplicationDbContext _applicationDbContext ;
    private readonly ILogger<ProductCategoryService> _iLogger ;

    public ProductCategoryService(IProductCategoryRepository iProductCategoryRepository, ApplicationDbContext applicationDbContext,  ILogger<ProductCategoryService> iLogger )
    {
        _iProductCategoryRepository = iProductCategoryRepository;
        _applicationDbContext = applicationDbContext;
        _iLogger = iLogger;
    }
    //tạo danh mục sản phẩm
    public ProductCategory InsertProductCategory(ProductCategory productCategory)
    {
        ProductCategory productParent = _iProductCategoryRepository.FindAll().Where(x => x.Id == productCategory.Pid)
            .FirstOrDefault();
        if (productParent == null && productCategory.Pid != null )
        {
            return null;
        }

        var index = _iProductCategoryRepository.FindAll().Where(x => x.Pid == productCategory.Pid).Max(x => x.Lft) ?? 0 + 1;
        var RgtNew = productCategory.Pid != null ? (productParent.Rgt + index) : index.ToString() ;
        ProductCategory newInsert = new ProductCategory()
        {
            Name = productCategory.Name,
            Font = productCategory.Font,
            Pid = productCategory.Pid,
            ImageBanner = productCategory.ImageBanner,
            NonName = productCategory.NonName,
            LastModifiedAt = productCategory.LastModifiedAt,
            LastModifiedBy = productCategory.LastModifiedBy,
            ImageBannerMobile = productCategory.ImageBannerMobile,
            Rgt = RgtNew,
            Lft = index,
            Lvl =  productCategory.Pid != null ? productParent.Lvl + 1 : 1
        };
        var rs = _iProductCategoryRepository.Create(newInsert);
        return rs;
    }   
    //update danh mục sản phẩm
    public ProductCategory UpdateProductCategory(ProductCategory productCategory)
    {
        ProductCategory productParent = _iProductCategoryRepository.FindAll().Where(x => x.Id == productCategory.Pid)
            .FirstOrDefault();
        if (productParent == null)
        {
            return null;
        }
        var index = _iProductCategoryRepository.FindAll().Where(x => x.Pid == productCategory.Pid).Max(x => x.Lft) ?? 0 + 1;
        var RgtNew = productParent.Rgt + index;
        productCategory.Rgt = RgtNew;
        productCategory.Lft = index;
        productCategory.Lvl = productParent.Lvl + 1;
       _iProductCategoryRepository.Update(productCategory);
       return productCategory;
    }

    public void UpdateOrder(List<int> ids, int parentId)
    {
        IDbContextTransaction transaction = _applicationDbContext.Database.BeginTransaction();
        try
        {
            ProductCategory parent = _iProductCategoryRepository.FindById(parentId);
            if (parent == null && parentId != 0)
            {
                return;
            }

            List<ProductCategory> listChildren =
                _iProductCategoryRepository.FindAll().Where(x => ids.Contains(x.Id)).ToList();
            listChildren.ForEach(item =>
            {
                int index = ids.FindIndex(x => x == item.Id);
                item.Lvl = parentId == 0 ? 1 : (parent.Lvl + 1);
                item.Rgt = (parentId == 0 ? "" : parent.Rgt )  + ( index + 1);
                item.Lft = index + 1;
                item.Pid = parentId == 0 ? null : parent.Id;
                UpdateChildrenOrder(item, ids);
            });
            _iProductCategoryRepository.BulkUpdate(listChildren);
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            this._iLogger.LogError(e, $"Update Db err : {e.Message}");
            throw;
        }
       
    }
    public void UpdateChildrenOrder(ProductCategory parent, List<int> ids)
    {
        List<ProductCategory> childrenMenus = _iProductCategoryRepository.FindAll().Where(x => x.Pid == parent.Id && !ids.Contains(x.Id) )
            .OrderBy(x => x.Lft).ToList();
        if (childrenMenus.Count < 1)
        {
            return;
        }
        int index = 1;
        childrenMenus.ForEach(item =>
        {
            item.Lvl = parent.Lvl + 1;
            item.Rgt = parent.Rgt + index;
            item.Lft = index;
            UpdateChildrenOrder(item, ids);
            index++;
        });
        _iProductCategoryRepository.BulkUpdate(childrenMenus);
    }
}