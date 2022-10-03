using System;
using System.Linq;
using CMS_Access.Repositories.Products;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using CMS.Areas.Products.Const;
using CMS.Areas.Products.Models.ProductCensorship;
using CMS.Controllers;
using CMS.Models.ModelContainner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npoi.Mapper;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Products.Controllers;

[Area("Products")]
[Obsolete]
public class ProductCensorshipController : BaseController
{
    private readonly ILogger<ProductCensorshipController> _iLogger;
    private readonly IProductRepository _iProductRepository;
    private readonly IProductPurposeRepository _iProductPurposeRepository;
    private readonly IProductCategoryRepository _iProductCategoryRepository;
    private readonly IProductImageRepository _iProductImageRepository;
    private readonly IProductSimilarRepository _iProductSimilarRepository;
    public ProductCensorshipController(ILogger<ProductCensorshipController> iLogger,
        IProductRepository iProductRepository, IProductPurposeRepository iProductPurposeRepository, IProductCategoryRepository iProductCategoryRepository, IProductImageRepository iProductImageRepository, IProductSimilarRepository iProductSimilarRepository)
    {
        _iLogger = iLogger;
        _iProductRepository = iProductRepository;
        _iProductPurposeRepository = iProductPurposeRepository;
        _iProductCategoryRepository = iProductCategoryRepository;
        _iProductImageRepository = iProductImageRepository;
        _iProductSimilarRepository = iProductSimilarRepository;
    }

    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Price(string txtSearch, int? status, int pageindex = 1)
    {
        var query = _iProductRepository.FindAll();
        if (!string.IsNullOrEmpty(txtSearch))
        {
            query = query.Where(x =>
                EF.Functions.Like(x.Name, "%" + txtSearch.Trim() + "%") ||
                EF.Functions.Like(x.Sku, "%" + txtSearch.Trim() + "%"));
        }

        if (status != null)
        {
            query = query.Where(x => x.Org1Status == status);
        }

        var listData = PagingList.Create(query.OrderByDescending(x => x.LastModifiedAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            {"txtSearch", txtSearch},
            {"status", status}
        };
        var model = new ModelCollection();
        model.AddModel("ListData", listData);
        model.AddModel("ListStatus", ProductCensorshipConst.ListStatus);
        model.AddModel("Page", pageindex);
        model.AddModel("statusProperties", "Org1Status");
        model.AddModel("commentProperties", "Org1Comment");
        model.AddModel("Title", "Duyệt giá, mã SP");
        return View("Index", model);
    }
    
    [NonLoad]
    [HttpPost]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductCensorshipController@Price")]
    public IActionResult ApprovedPrice(int id)
    {
        try
        {
            var product = _iProductRepository.FindById(id);
            if (product == null)
            {
                return Json(new
                {   code = 404,
                    msg = "Không tìm thấy sản phẩm"
                });
            }
            int approved = ProductCensorshipConst.Approved.Status;
            product.Org1Status = approved;
            product.IsPublic = product.Org1Status == approved && 
                               product.Org2Status == approved && 
                               product.Org3Status == approved;
            product.LastModifiedAt = DateTime.Now;
            product.LastModifiedBy = UserInfo.UserId;
            _iProductRepository.Update(product);
            ILoggingService.Infor(_iLogger, "Duyệt giá sản phẩm thành công",$"ProductId={id} UserId={UserInfo.UserId}");
            ToastMessage(1, "Duyệt giá sản phẩm thành công");
            return Json(new
            {   code = 200,
                msg = "Duyệt sản phẩm thành công"
            });
        }
        catch (Exception e)
        {
            ILoggingService.Error(_iLogger, "Duyệt giá sản phẩm thất bại",$"ProductId={id} UserId={UserInfo.UserId}, error={e.Message}");
            return Json(new
            {   code = 500,
                msg = "Duyệt sản phẩm thất bại"
            });
        }
    }
    
    [NonLoad]
    [HttpPost]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductCensorshipController@Price")]
    public IActionResult NotApprovedPrice([FromForm] CensorshipModel model)
    {
        if (!ModelState.IsValid)
        {
            var errorMessage = "" ;
            ModelState.Values.ForEach(v => v.Errors.ForEach( e => errorMessage += e.ErrorMessage + ". "));
            ToastMessage(-1, errorMessage);
            return RedirectToAction("Details",new{id=model.ProductId,backUrl=model.BackUrl});
        }
        try
        {
            var product = _iProductRepository.FindById(model.ProductId);
            if (product == null)
            {
                ToastMessage(-1, "Không tìm thấy sản phẩm !");
                return RedirectToAction("Details",new{id=model.ProductId,backUrl=model.BackUrl});
            }
            product.Org1Status = ProductCensorshipConst.NotApproved.Status;
            product.Org1Comment = model.Comment;
            product.IsPublic = false;
            product.LastModifiedAt = DateTime.Now;
            product.LastModifiedBy = UserInfo.UserId;
            _iProductRepository.Update(product);
            ILoggingService.Infor(_iLogger, "Yêu cầu chỉnh sửa giá sản phẩm thành công",$"ProductId={model.ProductId} UserId={UserInfo.UserId}");
            ToastMessage(1, "Yêu cầu chỉnh sửa giá sản phẩm thành công");
            return RedirectToAction("Details",new{id=model.ProductId,backUrl=model.BackUrl});
        }
        catch (Exception e)
        {
            ILoggingService.Error(_iLogger, "Yêu cầu chỉnh sửa giá sản phẩm thất bại",$"ProductId={model.ProductId} UserId={UserInfo.UserId}, error={e.Message}");
            ToastMessage(-1, "Yêu cầu chỉnh sửa giá sản phẩm thất bại");
            return RedirectToAction("Details",new{id=model.ProductId,backUrl=model.BackUrl});
        }
    }
    
    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Content(string txtSearch, int? status, int pageindex = 1)
    {
        var query = _iProductRepository.FindAll();
        if (!string.IsNullOrEmpty(txtSearch))
        {
            query = query.Where(x =>
                EF.Functions.Like(x.Name, "%" + txtSearch.Trim() + "%") ||
                EF.Functions.Like(x.Sku, "%" + txtSearch.Trim() + "%"));
        }

        if (status != null)
        {
            query = query.Where(x => x.Org2Status == status);
        }

        var listData = PagingList.Create(query.OrderByDescending(x => x.LastModifiedAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            {"txtSearch", txtSearch},
            {"status", status}
        };
        var model = new ModelCollection();
        model.AddModel("ListData", listData);
        model.AddModel("ListStatus", ProductCensorshipConst.ListStatus);
        model.AddModel("Page", pageindex);
        model.AddModel("statusProperties", "Org2Status");
        model.AddModel("commentProperties", "Org2Comment");
        model.AddModel("Title", "Duyệt nội dung SP");
        return View("Index", model);
    }
    
    [NonLoad]
    [HttpPost]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductCensorshipController@Content")]
    public IActionResult ApprovedContent(int id)
    {
        try
        {
            var product = _iProductRepository.FindById(id);
            if (product == null)
            {
                return Json(new
                {   code = 404,
                    msg = "Không tìm thấy sản phẩm"
                });
            }
            int approved = ProductCensorshipConst.Approved.Status;
            product.Org2Status = approved;
            product.IsPublic = product.Org1Status == approved && 
                               product.Org2Status == approved && 
                               product.Org3Status == approved;
            product.LastModifiedAt = DateTime.Now;
            product.LastModifiedBy = UserInfo.UserId;
            _iProductRepository.Update(product);
            ILoggingService.Infor(_iLogger, "Duyệt nội dung sản phẩm thành công",$"ProductId={id} UserId={UserInfo.UserId}");
            ToastMessage(1, "Duyệt nội dung sản phẩm thành công");
            return Json(new
            {   code = 200,
                msg = "Duyệt sản phẩm thành công"
            });
        }
        catch (Exception e)
        {
            ILoggingService.Error(_iLogger, "Duyệt nội dung phẩm thất bại",$"ProductId={id} UserId={UserInfo.UserId}, error={e.Message}");
            return Json(new
            {   code = 500,
                msg = "Duyệt sản phẩm thất bại"
            });
        }
    }
    
    [NonLoad]
    [HttpPost]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductCensorshipController@Content")]
    public IActionResult NotApprovedContent([FromForm]CensorshipModel model)
    {
        if (!ModelState.IsValid)
        {
            var errorMessage = "" ;
            ModelState.Values.ForEach(v => v.Errors.ForEach( e => errorMessage += e.ErrorMessage + ". "));
            ToastMessage(-1, errorMessage);
            return RedirectToAction("Details",new{id=model.ProductId,backUrl=model.BackUrl});
        }
        try
        {
            var product = _iProductRepository.FindById(model.ProductId);
            if (product == null)
            {
                ToastMessage(-1, "Không tìm thấy sản phẩm");
                return RedirectToAction("Details",new{id=model.ProductId,backUrl=model.BackUrl});
            }
            product.Org2Status = ProductCensorshipConst.NotApproved.Status;
            product.Org2Comment = model.Comment;
            product.IsPublic = false;
            product.LastModifiedAt = DateTime.Now;
            product.LastModifiedBy = UserInfo.UserId;
            _iProductRepository.Update(product);
            ILoggingService.Infor(_iLogger, "Yêu cầu chỉnh sửa nội dung sản phẩm thành công",$"ProductId={model.ProductId} UserId={UserInfo.UserId}");
            ToastMessage(1, "Yêu cầu chỉnh sửa nội dung sản phẩm thành công");
            return RedirectToAction("Details",new{id=model.ProductId,backUrl=model.BackUrl});
        }
        catch (Exception e)
        {
            ILoggingService.Error(_iLogger, "Yêu cầu chỉnh sửa nội dung sản phẩm thất bại",$"ProductId={model.ProductId} UserId={UserInfo.UserId}, error={e.Message}");
            ToastMessage(-1, "Yêu cầu chỉnh sửa nội dung sản phẩm thất bại");
            return RedirectToAction("Details",new{id=model.ProductId,backUrl=model.BackUrl});
        }
    }
    
    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Image(string txtSearch, int? status, int pageindex = 1)
    {
        var query = _iProductRepository.FindAll();
        if (!string.IsNullOrEmpty(txtSearch))
        {
            query = query.Where(x =>
                EF.Functions.Like(x.Name, "%" + txtSearch.Trim() + "%") ||
                EF.Functions.Like(x.Sku, "%" + txtSearch.Trim() + "%"));
        }

        if (status != null)
        {
            query = query.Where(x => x.Org3Status == status);
        }

        var listData = PagingList.Create(query.OrderByDescending(x => x.LastModifiedAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            {"txtSearch", txtSearch},
            {"status", status}
        };
        var model = new ModelCollection();
        model.AddModel("ListData", listData);
        model.AddModel("ListStatus", ProductCensorshipConst.ListStatus);
        model.AddModel("Page", pageindex);
        model.AddModel("statusProperties", "Org3Status");
        model.AddModel("commentProperties", "Org3Comment");
        model.AddModel("Title", "Duyệt hình ảnh, màu sắc, thương hiệu");
        return View("Index", model);
    }
    
    [NonLoad]
    [HttpPost]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductCensorshipController@Image")]
    public IActionResult ApprovedImage(int id)
    {
        try
        {
            var product = _iProductRepository.FindById(id);
            if (product == null)
            {
                return Json(new
                {   code = 404,
                    msg = "Không tìm thấy sản phẩm"
                });
            }

            int approved = ProductCensorshipConst.Approved.Status;
            product.Org3Status = approved;
            product.IsPublic = product.Org1Status == approved && 
                               product.Org2Status == approved && 
                               product.Org3Status == approved;
            product.LastModifiedAt = DateTime.Now;
            product.LastModifiedBy = UserInfo.UserId;
            _iProductRepository.Update(product);
            ILoggingService.Infor(_iLogger, "Duyệt hình ảnh, màu sắc, thương hiệu sản phẩm thành công",$"ProductId={id} UserId={UserInfo.UserId}");
            ToastMessage(1, "Duyệt hình ảnh, màu sắc, thương hiệu sản phẩm thành công");
            return Json(new
            {   code = 200,
                msg = "Duyệt sản phẩm thành công"
            });
        }
        catch (Exception e)
        {
            ILoggingService.Error(_iLogger, "Duyệt hình ảnh, màu sắc, thương hiệu sản phẩm thất bại",$"ProductId={id} UserId={UserInfo.UserId}, error={e.Message}");
            return Json(new
            {   code = 500,
                msg = "Duyệt sản phẩm thất bại"
            });
        }
    }
    
    [NonLoad]
    [HttpPost]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductCensorshipController@Image")]
    public IActionResult NotApprovedImage([FromForm]CensorshipModel model)
    {
        if (!ModelState.IsValid)
        {
            var errorMessage = "" ;
            ModelState.Values.ForEach(v => v.Errors.ForEach( e => errorMessage += e.ErrorMessage + ". "));
            ToastMessage(-1, errorMessage);
            return RedirectToAction("Details",new{id=model.ProductId,backUrl=model.BackUrl});
        }
        try
        {
            var product = _iProductRepository.FindById(model.ProductId);
            if (product == null)
            {
                ToastMessage(1, "Không tìm thấy sản phẩm");
                return RedirectToAction("Details",new{id=model.ProductId,backUrl=model.BackUrl});
            }
            product.Org3Status = ProductCensorshipConst.NotApproved.Status;
            product.Org3Comment = model.Comment;
            product.IsPublic = false;
            product.LastModifiedAt = DateTime.Now;
            product.LastModifiedBy = UserInfo.UserId;
            _iProductRepository.Update(product);
            ILoggingService.Infor(_iLogger, "Yêu cầu chỉnh sửa hình ảnh, màu sắc, thương hiệu sản phẩm thành công",$"ProductId={model.ProductId} UserId={UserInfo.UserId}");
            ToastMessage(1, "Yêu cầu chỉnh sửa hình ảnh, màu sắc, thương hiệu sản phẩm thành công");
            return RedirectToAction("Details",new{id=model.ProductId,backUrl=model.BackUrl});
        }
        catch (Exception e)
        {
            ILoggingService.Error(_iLogger, "Yêu cầu chỉnh sửa hình ảnh, màu sắc, thương hiệu phẩm thất bại",$"ProductId={model.ProductId} UserId={UserInfo.UserId}, error={e.Message}");
            ToastMessage(-1, "Yêu cầu chỉnh sửa sản phẩm thất bại");
            return RedirectToAction("Details",new{id=model.ProductId,backUrl=model.BackUrl});
        }
    }
    
    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Details(int id,string backUrl)
    {
        var model = new ModelCollection();
        var product = _iProductRepository.FindById(id);
        if (product == null)
        {
            return NotFound();
        }
        
        model.AddModel("Product", product);
        var purposeName = _iProductPurposeRepository.FindById(product.ProductPurposeId!.Value)?.Name ?? "";
        model.AddModel("Purpose", purposeName);
        var listCategory = _iProductCategoryRepository.GetListCategory(id).Select(x => x.Name).ToList();
        model.AddModel("Category", listCategory);
        var listImage = _iProductImageRepository.FindAll().Where(x => x.ProductId == id).ToList();
        model.AddModel("Images", listImage);
        var listPro = _iProductRepository.GetProductProperties(id);
        model.AddModel("ListPro",listPro );
        var listS = _iProductSimilarRepository.FindAll().Where(x => x.ProductId == id).ToList();
        model.AddModel("ListS", listS);
        model.AddModel("isCensorshipPrice", User.HasClaim(CmsClaimType.AreaControllerAction, "Products@ProductCensorshipController@Price".ToUpper()));
        model.AddModel("isCensorshipContent", User.HasClaim(CmsClaimType.AreaControllerAction, "Products@ProductCensorshipController@Content".ToUpper()));
        model.AddModel("isCensorshipImage", User.HasClaim(CmsClaimType.AreaControllerAction, "Products@ProductCensorshipController@Image".ToUpper()));
        backUrl = CmsFunction.IsBackUrl(backUrl) ? "#" : backUrl;
        backUrl = Url.IsLocalUrl(backUrl) ? backUrl : "#";
        model.AddModel("backUrl",backUrl);
        
        return View(model);
    }
}