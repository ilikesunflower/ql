using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Core.Internal;
using CMS.Areas.Categories.Const;
using CMS.Areas.Categories.Models.Article;
using CMS.Areas.Customer.Services;
using CMS.Controllers;
using CMS.Models;
using CMS.Models.ModelContainner;
using CMS_Access.Repositories.Categories;
using CMS_EF.Models.Articles;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Extensions.HtmlAgilityPack;
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
public class ArticleController : BaseController
{
    private readonly ILogger _iLogger;
    private readonly IArticleRepository _iArticleRepository;
    private readonly IArticleTypeRepository _iArticleTypeRepository;
    private readonly ICustomerNotificationService _iCustomerNotificationService;

    public ArticleController(ILogger<ArticleController> iLogger, IArticleRepository iArticleRepository,
        IArticleTypeRepository articleTypeRepository, ICustomerNotificationService iCustomerNotificationService)
    {
        _iLogger = iLogger;
        _iArticleRepository = iArticleRepository;
        _iArticleTypeRepository = articleTypeRepository;
        _iCustomerNotificationService = iCustomerNotificationService;
    }

    // GET
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Index(string txtKeyword, string startDate, int? type, string endDate, int? status,
        int pageindex = 1)
    {
        var query = _iArticleRepository.FindAll();
        if (!txtKeyword.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Title, "%" + txtKeyword.Trim() + "%"));
        }

        if (type.HasValue)
        {
            query = query.Where(x => x.ArticleType == type);
        }

        if (!string.IsNullOrEmpty(startDate))
        {
            var start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.LastModifiedAt >= start);
        }

        if (!string.IsNullOrEmpty(endDate))
        {
            var end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.LastModifiedAt <= end);
        }

        if (status != null && status != 0)
        {
            switch (status)
            {
                case 1:
                    query = query.Where(x => x.Status == 1);
                    break;
                case 2:
                    query = query.Where(x => x.Status == 0);
                    break;
            }
        }

        var listData = PagingList.Create(query.OrderByDescending(x => x.LastModifiedAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            { "txtKeyword", txtKeyword },
            { "startDate", startDate },
            { "endDate", endDate },
            { "type", type },
            { "status", status }
        };
        ModelCollection model = new ModelCollection();
        model.AddModel("ListData", listData);
        model.AddModel("Page", pageindex);
        model.AddModel("ListArticleType", this._iArticleTypeRepository.FindAll().ToList());
        return View(model);
    }

    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Create()
    {
        CreateModel model = new CreateModel();
        model.ListArticleType = _iArticleTypeRepository.FindAll().ToList();
        model.StatusBox = true;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Categories@ArticleController@Create")]
    [NonLoad]
    public IActionResult Create(CreateModel createData)
    {
        if (ModelState.IsValid)
        {
            try
            {
                Article article = new Article
                {
                    Title = createData.Title.Trim(),
                    Thumbnail = createData.Thumbnail,
                    Lead = createData.Lead == null ? null : HtmlAgilityPackService.DeleteBase64(createData.Lead),
                    Detail = createData.Detail == null ? null : HtmlAgilityPackService.DeleteBase64(createData.Detail),
                    Status = createData.StatusBox ? 1 : 0,
                    IsHot = createData.IsHot,
                    ArticleType = createData.ArticleType,
                    Author = createData.Author,
                    LastModifiedAt = DateTime.Now,
                    LastModifiedBy = UserInfo.UserId
                };


                var postDate = CmsFunction.ConvertStringToDateTimeH(createData.PostDate);
                if (postDate != null)
                {
                    article.PublishTime = postDate.Value;
                }

                var res = _iArticleRepository.Create(article);

                if (res != null)
                {
                    ToastMessage(1, "Thêm mới tin bài thành công");
                    ILoggingService.Infor(_iLogger, "Thêm mới tin bài thành công", "id:" + res.Id);
                    return RedirectToAction(nameof(Details), new { id = res.Id });
                }

                ILoggingService.Error(_iLogger, "Thêm mới tin bài lỗi");
            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "Thêm mới tin bài lỗi: " + e.Message);
            }

            ToastMessage(-1, "Thêm mới tin tức lỗi, liên hệ người quản trị");
        }

        createData.ListArticleType = _iArticleTypeRepository.FindAll().ToList();
        return View(createData);
    }

    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Details(int id)
    {
        ModelCollection model = new ModelCollection();
        var article = _iArticleRepository.FindById(id);
        if (article == null)
        {
            return NotFound();
        }
        model.AddModel("Article", article);
        var articleType = _iArticleTypeRepository.FindAll().ToList();
        model.AddModel("ListArticleType", articleType);
        model.AddModel("IsSendNotification", article.Status == StatusConst.ArticleShow && User.HasClaim(CmsClaimType.AreaControllerAction,"Categories@ArticleController@SendNotification".ToUpper()));
        return View(model);
    }

    [HttpPost]
    [Authorize(Policy = "PermissionMVC")]
    public JsonResult Delete(int? id)
    {
        if (id == null)
        {
            ToastMessage(-1, "Không có dữ liệu tin tức");
            return Json(new
            {
                msg = "fail",
                content = "Không có dữ liệu, không thể xóa"
            });
        }

        try
        {
            var company = _iArticleRepository.FindById(id.Value);
            if (company != null)
            {
                _iArticleRepository.Delete(company);
                ILoggingService.Infor(this._iLogger, "Xóa tin tức thành công id:" + id, "UserId: " + UserInfo.UserId);
                ToastMessage(1, "Xóa tin tức thành công");
                return Json(new
                {
                    msg = "successful",
                    content = "Xóa tin tức thành công"
                });
            }
            else
            {
                ILoggingService.Error(this._iLogger, "Xóa tin tức lỗi" + "id:" + id, "UserId: " + UserInfo.UserId);
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
            ILoggingService.Error(this._iLogger, "Xóa tin tức lỗi" + "id:" + id, "UserId: " + UserInfo.UserId, ex);
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
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Categories@ArticleController@Delete")]
    [NonLoad]
    public JsonResult DeleteAll(List<int> id)
    {
        if (id == null)
        {
            ToastMessage(-1, "Không có dữ liệu tin tức");
            return Json(new
            {
                msg = "fail",
                content = "Không có dữ liệu, không thể xóa"
            });
        }

        try
        {
            int rs = this._iArticleRepository.DeleteAll(id);
            ToastMessage(1, $"Xóa thành công {rs} bản ghi");
            this._iLogger.LogInformation($"Xóa thành công {rs} tin tức, UserId: " + UserInfo.UserId);
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

    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var article = _iArticleRepository.FindById(id.Value);
        if (article == null)
        {
            return NotFound();
        }

        EditModel model = new EditModel
        {
            Title = article.Title,
            Thumbnail = article.Thumbnail,
            Lead = article.Lead,
            Detail = article.Detail,
            StatusBox = article.Status.Value == 1,
            IsHot = article.IsHot.Value,
            ArticleType = article.ArticleType,
            Author = article.Author,
            PostDate = article.PublishTime.ToString("dd/MM/yyyy HH:mm"),
        };
        model.ListArticleType = _iArticleTypeRepository.FindAll().ToList();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Categories@ArticleController@Edit")]
    [NonLoad]
    public IActionResult Edit(EditModel EditData)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var article = _iArticleRepository.FindById(EditData.Id);
                article.Title = EditData.Title.Trim();
                article.Author = EditData.Author;
                article.Thumbnail = EditData.Thumbnail;
                article.Lead = EditData.Lead == null ? null : HtmlAgilityPackService.DeleteBase64(EditData.Lead);
                article.Detail = EditData.Detail == null ? null : HtmlAgilityPackService.DeleteBase64(EditData.Detail);
                article.Status = EditData.StatusBox ? 1 : 0;
                article.IsHot = EditData.IsHot;
                article.ArticleType = EditData.ArticleType;
                var postDate = CmsFunction.ConvertStringToDateTimeH(EditData.PostDate);
                if (postDate != null)
                {
                    article.PublishTime = postDate.Value;
                }

                _iArticleRepository.Update(article);
                ILoggingService.Infor(this._iLogger, "Chỉnh sửa tin tức thành công", "id:" + EditData.Id);
                ToastMessage(1, "Chỉnh sửa tin tức thành công");
                return RedirectToAction(nameof(Details), new { EditData.Id });
            }
            else
            {
                return View(EditData);
            }
        }
        catch (Exception e)
        {
            ILoggingService.Error(this._iLogger, "Chỉnh sửa tin tức lỗi" + "id:" + EditData.Id,
                "UserId :" + UserInfo.UserId, e);
            ToastMessage(-1, "Lỗi không thể sửa tin tức này, Vui lòng liên hệ người quản trị");
        }

        return View(EditData);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult SendNotification(int id)
    {
        try
        {
            var article = this._iArticleRepository.FindById(id);
            if (article != null)
            {
                this._iCustomerNotificationService.SendCustomerNotification(new CustomerNotificationObject()
                {
                    Title = article.Title,
                    Detail = article.Lead,
                    Link =$"/{CmsFunction.RewriteUrlFriendly(article.Title)}-{article.Id}.html"
                });
                return Ok(new OutputObject(200,"","").Show());
            }
            else
            {
                return Ok(new OutputObject(400,"","Không có nội dung").Show());
            }
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex, "Gửi thông báo tin bài thành công");
            return Ok(new OutputObject(400,"","").Show());
        }
    }
}