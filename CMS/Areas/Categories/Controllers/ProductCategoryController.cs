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
    [HttpGet]
    public  Task<IActionResult> Index(string txtSearch, int? status, int? pid)
    {
        
        var query = _iProductCategoryRepository.FindAll();
        if (!txtSearch.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Name, "%" + txtSearch.Trim() + "%"));
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
        rs.AddModel("txtSearch", txtSearch);
        rs.AddModel("pid", pid);
        rs.AddModel("ListData", listData);
        rs.AddModel("ListCategoryProduct",
            _iProductCategoryRepository.GetListProductCategory());
        ILoggingService.Infor(_iLogger, "Xem danh s??ch danh m???c s???n ph???m");
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
                    ToastMessage(-1, "T??n danh m???c s???n ph???m ???? t???n t???i");
                    return View(createData);
                }

                var res = _iProductCategoryService.InsertProductCategory(productCategory);

                if (res != null)
                {
                    ToastMessage(1, "Th??m m???i danh m???c s???n ph???m th??nh c??ng");
                    ILoggingService.Infor(_iLogger, "Th??m m???i danh m???c s???n ph???m th??nh c??ng", "id:" + res.Id);
                     return RedirectToAction(nameof(Details), new { id = res.Id });
                }

                ILoggingService.Error(_iLogger, "Th??m m???i danh m???c s???n ph???m l???i");
            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "Th??m m???i danh m???c s???n ph???m l???i: " + e.Message);
            }
            ToastMessage(-1, "Th??m m???i danh m???c s???n ph???m l???i, li??n h??? ng?????i qu???n tr???");
        }
        createData.ListCategories = _iProductCategoryRepository.GetListProductCategory();
        createData.ImageBanner = CmsFunction.IsValidImage(createData.ImageBanner) ? "" : createData.ImageBanner;
        createData.ImageBannerMobile = CmsFunction.IsValidImage(createData.ImageBannerMobile) ? "" : createData.ImageBannerMobile;
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
                    ToastMessage(-1, "T??n danh m???c s???n ph???m ???? t???n t???i");
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
                        ILoggingService.Error(this._iLogger, "S???a danh m???c s???n ph???m l???i: ");
                        ToastMessage(-1, "S???a danh m???c s???n ph???m l???i, li??n h??? ng?????i qu???n tr???");
                        editData.ListCategories = _iProductCategoryRepository.GetListProductCategory()
                            .Where(x => x.Id != editData.Id).ToList();
                        return View(editData);
                    }
                }
                ILoggingService.Error(_iLogger, "S???a danh m???c s???n ph???m th??nh c??ng");
                ToastMessage(1, "S???a danh m???c s???n ph???m th??nh c??ng");
                return RedirectToAction(nameof(Details), new { id = editData.Id });
            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "S???a danh m???c s???n ph???m l???i: " + e.Message);
            }
            ToastMessage(-1, "S???a danh m???c s???n ph???m l???i, li??n h??? ng?????i qu???n tr???");
        }
        editData.ListCategories = _iProductCategoryRepository.GetListProductCategory()
            .Where(x => x.Id != editData.Id).ToList();
        editData.ImageBanner = CmsFunction.IsValidImage(editData.ImageBanner) ? "" : editData.ImageBanner;
        editData.ImageBannerMobile = CmsFunction.IsValidImage(editData.ImageBannerMobile) ? "" : editData.ImageBannerMobile;
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
    [ValidateAntiForgeryToken]
    // [Authorize(Policy = "PermissionMVC")]
    [NonLoad]
    public JsonResult UpdateOrder(UpdateOrderViewModel form)
    {
        try
        {
            _iProductCategoryService.UpdateOrder(form.Ids,form.Parent);
            ILoggingService.Infor(_iLogger, "C???p nh???t tr???ng th??i danh m???c s???n ph???m th??nh c??ng");
            ToastMessage(1,"C???p nh???t v??? tr?? danh m???c s???n ph???m th??nh c??ng");
            return Json(new
            {
                msg = "successful",
                content = "C???p nh???t v??? tr?? danh m???c s???n ph???m th??nh c??ng"
            });
        }
        catch (Exception e)
        {
            ILoggingService.Error(_iLogger, "C???p nh???t tr???ng th??i danh m???c s???n ph???m l???i", e.ToString(), e);
            return Json(new
            {
                msg = "fail",
                content = "C???p nh???t v??? tr?? danh m???c s???n ph???m l???i: " + e
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
                ToastMessage(-1, "Kh??ng c?? d??? li???u danh m???c s???n ph???m");
                return Json(new
                {
                    msg = "fail",
                    content = "Kh??ng c?? d??? li???u, kh??ng th??? x??a"
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
                    ILoggingService.Infor(this._iLogger, "X??a danh m???c s???n ph???m th??nh c??ng id:" + id, "UserId: " + UserInfo.UserId);
                    ToastMessage(1, "X??a danh m???c s???n ph???m th??nh c??ng");
                    return Json(new
                    {
                        msg = "successful",
                        content = "X??a danh m???c s???n ph???m th??nh c??ng"
                    });
                }
                else
                {
                    ILoggingService.Error(this._iLogger, "X??a danh m???c s???n ph???m l???i" + "id:" + id, "UserId: " + UserInfo.UserId);
                    ToastMessage(-1, "X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Kh??ng c?? d??? li???u, kh??ng th??? x??a"
                    });
                }
            }
            catch (Exception ex)
            {
                ILoggingService.Error(this._iLogger, "X??a danh m???c s???n ph???m l???i" + "id:" + id, "UserId: " + UserInfo.UserId, ex);
                ToastMessage(-1, "X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???");
                return Json(new
                {
                    msg = "fail",
                    content = "L???i kh??ng th??? x??a b???n ghi n??y, vui l??ng nh???p li??n h??? ng?????i qu???n tr???"
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
                ToastMessage(-1, "Kh??ng c?? d??? li???u banner");
                return Json(new
                {
                    msg = "fail",
                    content = "Kh??ng c?? d??? li???u, kh??ng th??? x??a"
                });
            }

            try
            {
                int rs = this._iProductCategoryRepository.DeleteAll(id);
                var productCategoryValue = _iProductCategoryProductRepository.FindAll()
                    .Where(x => id.Contains(x.PcategoryId.Value)).ToList();
                _iProductCategoryProductRepository.DeleteAll(productCategoryValue);
                ToastMessage(1, $"X??a th??nh c??ng {rs} b???n ghi");
                this._iLogger.LogInformation($"X??a th??nh c??ng {rs} danh m???c s???n ph???m, UserId: " + UserInfo.UserId);
                return Json(new
                {
                    msg = "successful",
                    content = ""
                });
            }
            catch (Exception)
            {
                ToastMessage(-1, "X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???");
                this._iLogger.LogError($"X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???: id {id} , UserId: {UserInfo.UserId}");
                return Json(new
                {
                    msg = "fail",
                    content = "L???i kh??ng th??? x??a b???n ghi n??y, vui l??ng nh???p li??n h??? ng?????i qu???n tr???"
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
                ILoggingService.Infor(_iLogger, "C???p nh???t tr???ng th??i trang danh s??ch s???n ph???m danh m???c th??nh c??ng");
                ToastMessage(1,"C???p nh???t v??? tr?? danh s??ch s???n ph???m thu???c danh m???c th??nh c??ng");
                return Json(new
                {
                    msg = "successful",
                    content = "C???p nh???t v??? tr?? s???n ph???m danh m???c th??nh c??ng"
                });
            }
            catch (Exception e)
            {
                ILoggingService.Error(_iLogger, "C???p nh???t tr???ng th??i s???n ph???m danh m???c l???i", e.ToString(), e);
                return Json(new
                {
                    msg = "fail",
                    content = "C???p nh???t v??? tr?? menu l???i: " + e
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
                ToastMessage(-1, "Kh??ng c?? d??? li???u danh m???c s???n ph???m");
                return Json(new
                {
                    msg = "fail",
                    content = "Kh??ng c?? d??? li???u, kh??ng th??? x??a"
                });
            }

            try
            {
                var productCategory = _iProductCategoryProductRepository.FindById(id.Value);
                if (productCategory != null)
                {
                    _iProductCategoryProductRepository.Delete(productCategory);
                    ILoggingService.Infor(this._iLogger, "X??a s???n ph???m c???a danh m???c th??nh c??ng id:" + id, "UserId: " + UserInfo.UserId);
                    ToastMessage(1, "X??a s???n ph???m c???a danh m???c th??nh c??ng");
                    return Json(new
                    {
                        msg = "successful",
                        content = "X??a s???n ph???m c???a danh m???c th??nh c??ng"
                    });
                }
                else
                {
                    ILoggingService.Error(this._iLogger, "X??a s???n ph???m c???a danh m???c l???i" + "id:" + id, "UserId: " + UserInfo.UserId);
                    ToastMessage(-1, "X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Kh??ng c?? d??? li???u, kh??ng th??? x??a"
                    });
                }
            }
            catch (Exception ex)
            {
                ILoggingService.Error(this._iLogger, "X??a s???n ph???m c???a danh m???c l???i" + "id:" + id, "UserId: " + UserInfo.UserId, ex);
                ToastMessage(-1, "X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???");
                return Json(new
                {
                    msg = "fail",
                    content = "L???i kh??ng th??? x??a b???n ghi n??y, vui l??ng nh???p li??n h??? ng?????i qu???n tr???"
                });
            }
        }
}

