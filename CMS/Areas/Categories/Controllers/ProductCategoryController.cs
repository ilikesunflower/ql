using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Castle.Core.Internal;
using CMS.Areas.Categories.Models.ProductCategory;
using CMS.Areas.Categories.Services;
using CMS.Areas.Products.Services;
using CMS.Controllers;
using CMS.Models.ModelContainner;
using CMS_Access.Repositories.Categories;
using CMS_Access.Repositories.Products;
using CMS_EF.Models;
using CMS_EF.Models.Products;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Categories.Controllers;

[Area("Categories")]
[Obsolete]

public class ProductCategoryController : BaseController
{
    private readonly ILogger _iLogger;
    private readonly IProductCategoryRepository _iProductCategoryRepository;
    private readonly IProductCategoryService _iProductCategoryService;
    private readonly IProductCategoryProductRepository _iProductCategoryProductRepository;
    public ProductCategoryController(ILogger<BannerController> iLogger, IProductCategoryRepository iProductCategoryRepository, IProductCategoryProductRepository iProductCategoryProductRepository,
        IProductCategoryService iProductCategoryService)
    {
        _iLogger = iLogger;
        _iProductCategoryRepository = iProductCategoryRepository;
        _iProductCategoryProductRepository = iProductCategoryProductRepository;
        _iProductCategoryService = iProductCategoryService;
    }
    // GET
    [Authorize(Policy = "PermissionMVC")]
    public  Task<IActionResult> Index(string txtKeyword, int? status, int? pid)
    {
        
        var query = _iProductCategoryRepository.FindAll();
        if (!txtKeyword.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Name, "%" + txtKeyword.Trim() + "%"));
        }
        if (pid.HasValue)
        {
            var pData = _iProductCategoryRepository.FindById(pid.Value);
            if (pData != null)
            {
                query = query.Where(x =>  x.Rgt.StartsWith(pData.Rgt) );
            }
        }
        List<ProductCategory> listData = query.OrderBy(x => x.Lft).ToList();
         
        ModelCollection rs = new ModelCollection();
        rs.AddModel("txtKeyword", txtKeyword);
        rs.AddModel("pid", pid);
        rs.AddModel("ListData", listData);
        rs.AddModel("ListCategoryProduct",
            _iProductCategoryRepository.GetListProductCategory());
        ILoggingService.Infor(_iLogger, "Xem danh sách danh mục sản phẩm");
        return Task.FromResult<IActionResult>(View(rs));
    }
    
    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Create()
    {
        var model = new CreateModel();
        model.ListCategories = _iProductCategoryRepository.GetListProductCategory();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Categories@ProductCategoryController@Create")]
    [NonLoad]
    public IActionResult Create(CreateModel createData)
    {

        if (ModelState.IsValid)
        {
            try
            {
                ProductCategory productCategory = new ProductCategory
                {
                    Name = createData.Name.Trim(),
                    Font = createData.Font,
                    Pid = createData.Pid ,
                    ImageBanner = createData.ImageBanner,
                    NonName =CmsFunction.RemoveUnicode(createData.Name.Trim()),
                    LastModifiedAt = DateTime.Now,
                    LastModifiedBy = UserInfo.UserId,
                    ImageBannerMobile = createData.ImageBannerMobile
                };
                var check = _iProductCategoryRepository.FindAll().Where(x => x.NonName == productCategory.NonName)
                    .FirstOrDefault();
                if (check != null)
                {
                    ToastMessage(-1, "Tên danh mục sản phẩm đã tồn tại");
                    return View(createData);
                }

                var res = _iProductCategoryService.InsertProductCategory(productCategory);

                if (res != null)
                {
                    ToastMessage(1, "Thêm mới danh mục sản phẩm thành công");
                    ILoggingService.Infor(_iLogger, "Thêm mới danh mục sản phẩm thành công", "id:" + res.Id);
                     return RedirectToAction(nameof(Details), new { id = res.Id });
                }

                ILoggingService.Error(_iLogger, "Thêm mới danh mục sản phẩm lỗi");
            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "Thêm mới danh mục sản phẩm lỗi: " + e.Message);
            }
            ToastMessage(-1, "Thêm mới danh mục sản phẩm lỗi, liên hệ người quản trị");
        }
        createData.ListCategories = _iProductCategoryRepository.GetListProductCategory();
        return View(createData);
    }
    
        
    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Edit(int id)
    {
        var productCategory = _iProductCategoryRepository.FindById(id);
        var model = new EditModel()
        {
            Id = id,
            Name = productCategory.Name,
            Font = productCategory.Font,
            ImageBanner = productCategory.ImageBanner,
            ImageBannerMobile = productCategory.ImageBannerMobile,
            Pid = productCategory.Pid,
            ListCategories = _iProductCategoryRepository.GetListProductCategory().Where(x => x.Id != id).ToList()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Categories@ProductCategoryController@Edit")]
    [NonLoad]
    public IActionResult Edit(EditModel editData)
    {

        if (ModelState.IsValid)
        {
            try
            {
             
                var producCategory = _iProductCategoryRepository.FindById(editData.Id);

                producCategory.Name = editData.Name.Trim();
                producCategory.NonName =CmsFunction.RemoveUnicode(editData.Name.Trim());
                producCategory.Font = editData.Font;
                producCategory.ImageBanner = editData.ImageBanner;
                producCategory.ImageBannerMobile = editData.ImageBannerMobile;
                var check = _iProductCategoryRepository.FindAll().Where(x => x.NonName ==  producCategory.NonName && x.Id != producCategory.Id )
                    .FirstOrDefault();
                if (check != null)
                {
                    ToastMessage(-1, "Tên danh mục sản phẩm đã tồn tại");
                    return View(editData);
                }

                if (editData.Pid == producCategory.Pid)
                {
                    _iProductCategoryRepository.Update(producCategory);
                }
                else
                {
                    producCategory.Pid = editData.Pid;
                    var rs = _iProductCategoryService.UpdateProductCategory(producCategory);
                    if (rs == null)
                    {
                        ILoggingService.Error(this._iLogger, "Sửa danh mục sản phẩm lỗi: ");
                        ToastMessage(-1, "Sửa danh mục sản phẩm lỗi, liên hệ người quản trị");
                        editData.ListCategories = _iProductCategoryRepository.GetListProductCategory()
                            .Where(x => x.Id != editData.Id).ToList();
                        return View(editData);
                    }
                }
                ILoggingService.Error(_iLogger, "Sửa danh mục sản phẩm thành công");
                ToastMessage(1, "Sửa danh mục sản phẩm thành công");
                return RedirectToAction(nameof(Details), new { id = editData.Id });
            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "Sửa danh mục sản phẩm lỗi: " + e.Message);
            }
            ToastMessage(-1, "Sửa danh mục sản phẩm lỗi, liên hệ người quản trị");
        }
        editData.ListCategories = _iProductCategoryRepository.GetListProductCategory()
            .Where(x => x.Id != editData.Id).ToList();
        return View(editData);
    }
    
    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Details(int id)
    {
        ProductCategory productCategory = _iProductCategoryRepository.FindById(id);
        DetailModel model = new DetailModel()
        {
            ProductCategory = productCategory,
            Parent = _iProductCategoryRepository.FindAll().Where(x => x.Id == (productCategory.Pid ?? 0))
                .Select(x => x.Name).FirstOrDefault()

        };
        return View(model);
    }
    [HttpPost]
    // [ValidateAntiForgeryToken]
    // [Authorize(Policy = "PermissionMVC")]
    public JsonResult UpdateOrder(UpdateOrderViewModel form)
    {
        try
        {
            _iProductCategoryService.UpdateOrder(form.Ids,form.Parent);
            ILoggingService.Infor(_iLogger, "Cập nhật trạng thái danh mục sản phẩm thành công");
            ToastMessage(1,"Cập nhật vị trí danh mục sản phẩm thành công");
            return Json(new
            {
                msg = "successful",
                content = "Cập nhật vị trí danh mục sản phẩm thành công"
            });
        }
        catch (Exception e)
        {
            ILoggingService.Error(_iLogger, "Cập nhật trạng thái danh mục sản phẩm lỗi", e.ToString(), e);
            return Json(new
            {
                msg = "fail",
                content = "Cập nhật vị trí danh mục sản phẩm lỗi: " + e
            });
        }
    }

    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult ViewCategoryProduct(int id,  int pageindex = 1)
    {
        ModelCollection model = new ModelCollection();
        var productCate = _iProductCategoryRepository.FindById(id)!.Name;
        var productCategory = _iProductCategoryRepository.GetListProductOrder(id);
        var listData = PagingList.Create(productCategory.OrderBy(x => x.Ord), PageSize, pageindex);
        model.AddModel("ListData", listData);
        model.AddModel("Id", id);
        model.AddModel("Page", pageindex);
        model.AddModel("NameCategory", productCate);
        return View(model);
    }
    
        [HttpPost]
        [Authorize(Policy = "PermissionMVC")]
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu danh mục sản phẩm");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                var productCategory = _iProductCategoryRepository.FindById(id.Value);
                if (productCategory != null)
                {
                    _iProductCategoryRepository.Delete(productCategory);
                    var productCategoryValue = _iProductCategoryProductRepository.FindAll()
                        .Where(x => x.PcategoryId == id.Value).ToList();
                    _iProductCategoryProductRepository.DeleteAll(productCategoryValue);
                    ILoggingService.Infor(this._iLogger, "Xóa danh mục sản phẩm thành công id:" + id, "UserId: " + UserInfo.UserId);
                    ToastMessage(1, "Xóa danh mục sản phẩm thành công");
                    return Json(new
                    {
                        msg = "successful",
                        content = "Xóa danh mục sản phẩm thành công"
                    });
                }
                else
                {
                    ILoggingService.Error(this._iLogger, "Xóa danh mục sản phẩm lỗi" + "id:" + id, "UserId: " + UserInfo.UserId);
                    ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Không có dữ liệu, không thể xóa"
                    });
                }
            }
            catch (Exception ex)
            {
                ILoggingService.Error(this._iLogger, "Xóa danh mục sản phẩm lỗi" + "id:" + id, "UserId: " + UserInfo.UserId, ex);
                ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Categories@ProductCategoryController@Delete")]
        [NonLoad]
        public JsonResult DeleteAll(List<int> id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu banner");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                int rs = this._iProductCategoryRepository.DeleteAll(id);
                var productCategoryValue = _iProductCategoryProductRepository.FindAll()
                    .Where(x => id.Contains(x.PcategoryId.Value)).ToList();
                _iProductCategoryProductRepository.DeleteAll(productCategoryValue);
                ToastMessage(1, $"Xóa thành công {rs} bản ghi");
                this._iLogger.LogInformation($"Xóa thành công {rs} danh mục sản phẩm, UserId: " + UserInfo.UserId);
                return Json(new
                {
                    msg = "successful",
                    content = ""
                });
            }
            catch (Exception)
            {
                ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                this._iLogger.LogError($"Xóa dữ liệu lỗi, liên hệ người quản trị: id {id} , UserId: {UserInfo.UserId}");
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                });
            }
        }
        
        [HttpPost]
        [NonLoad]
        public JsonResult UpdateOrderCategory(OrderCategoryModel form)
        {
            try
            {
                if (form.Ids != null && form.Ids.Count > 0)
                {
                  
                        for (int i = 0; i < form.Ids.Count; i++)
                        {
                            var lll = form.Ids[i];
                                var productCategoryValue = _iProductCategoryProductRepository.FindById(form.Ids[i]);
                                productCategoryValue.Ord = form.Ords[i];
                                _iProductCategoryProductRepository.Update(productCategoryValue);
                           
                         
                        }
                }
                ILoggingService.Infor(_iLogger, "Cập nhật trạng thái trang danh sách sản phẩm danh mục thành công");
                ToastMessage(1,"Cập nhật vị trí danh sách sản phẩm thuộc danh mục thành công");
                return Json(new
                {
                    msg = "successful",
                    content = "Cập nhật vị trí sản phẩm danh mục thành công"
                });
            }
            catch (Exception e)
            {
                ILoggingService.Error(_iLogger, "Cập nhật trạng thái sản phẩm danh mục lỗi", e.ToString(), e);
                return Json(new
                {
                    msg = "fail",
                    content = "Cập nhật vị trí menu lỗi: " + e
                });
            }
        }
        
        
        [HttpPost]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Categories@ProductCategoryController@ViewCategoryProduct")]
        [NonLoad]
        public JsonResult DeleteProduct(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu danh mục sản phẩm");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                var productCategory = _iProductCategoryProductRepository.FindById(id.Value);
                if (productCategory != null)
                {
                    _iProductCategoryProductRepository.Delete(productCategory);
                    ILoggingService.Infor(this._iLogger, "Xóa sản phẩm của danh mục thành công id:" + id, "UserId: " + UserInfo.UserId);
                    ToastMessage(1, "Xóa sản phẩm của danh mục thành công");
                    return Json(new
                    {
                        msg = "successful",
                        content = "Xóa sản phẩm của danh mục thành công"
                    });
                }
                else
                {
                    ILoggingService.Error(this._iLogger, "Xóa sản phẩm của danh mục lỗi" + "id:" + id, "UserId: " + UserInfo.UserId);
                    ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Không có dữ liệu, không thể xóa"
                    });
                }
            }
            catch (Exception ex)
            {
                ILoggingService.Error(this._iLogger, "Xóa sản phẩm của danh mục lỗi" + "id:" + id, "UserId: " + UserInfo.UserId, ex);
                ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                });
            }
        }
}

