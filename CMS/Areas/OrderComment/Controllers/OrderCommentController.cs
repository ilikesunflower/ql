using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Core.Internal;
using CMS_Access.Repositories.Orders;
using CMS_Access.Repositories.Products;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using CMS.Areas.OrderComment.Models;
using CMS.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.OrderComment.Controllers;
[Area("OrderComment")]
[Obsolete]
public class OrderCommentController : BaseController
{
    // GET
    private readonly ILogger _iLogger;
    private readonly IOrderProductRateCommentRepository _iOrderCommentRepository;
    private readonly IProductRepository _iProductRepository;
    public OrderCommentController(ILogger<OrderCommentController> iLogger, IOrderProductRateCommentRepository iOrderCommentRepository, IProductRepository iProductRepository)
    {
        _iLogger = iLogger;
        _iOrderCommentRepository = iOrderCommentRepository;
        _iProductRepository = iProductRepository;
    }
    
    [Authorize(Policy = "PermissionMVC")]
    [HttpGet]
    public IActionResult Index(string txtSearch, string startDate, string endDate, int? status, int pageindex = 1)
    {
        try
        {
            var query = _iOrderCommentRepository.GetAllProductComment();
            if (!txtSearch.IsNullOrEmpty())
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.Customer.FullName, "%" + txtSearch.Trim() + "%")
                    || 
                    EF.Functions.Like(x.Customer.UserName, "%" + txtSearch.Trim() + "%")
                    ||
                    EF.Functions.Like(x.Comment, "%" + txtSearch.Trim() + "%")
                    ||
                    EF.Functions.Like(x.Product.Name, "%" + txtSearch.Trim() + "%")
                    ||
                    EF.Functions.Like(x.Orders.Code, "%" + txtSearch.Trim() + "%")
                );
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt",
                    CultureInfo.InvariantCulture);
                query = query.Where(x => x.CreatedAt >= start);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                var end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt",
                    CultureInfo.InvariantCulture);
                query = query.Where(x => x.CreatedAt <= end);
            }
            if (status != null && status != 0)
            {
                switch (status)
                {
                    case 1:
                        query = query.Where(x => x.Status.Value );
                        break;
                    case 2:
                        query = query.Where(x => !x.Status.Value);
                        break;
                }
            }

            var listData = PagingList.Create(query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Orders.Code), PageSize, pageindex);
            var orderList = listData.ToList().Select(x => x.Orders).ToList();
            var dataOrder = orderList.GroupBy(x => x.Code).Select(x => new CommnentOrder
            {
                Code = x.Key,
                Count = x.Count()
            }).ToList();
            listData.RouteValue = new RouteValueDictionary()
            {
                {"txtSearch", txtSearch},
                {"startDate", startDate},
                {"endDate", endDate},
                {"status", status}
            };
            CommentIndexModel model = new CommentIndexModel();
            model.ListData = listData;
            model.Page = pageindex;
            model.ListOrder = dataOrder;
            return View(model);
        }
        catch (Exception e)
        {
            _iLogger.LogError(e,$"Danh sách đánh giá");
            return View();
        }
       
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "PermissionMVC")]
    public JsonResult ChangeStatus(int id, int status)
    {
        try
        {
            _iOrderCommentRepository.ChangeStatus(id, status == 1);
            ILoggingService.Infor(this._iLogger, "Cập nhật trạng thái đánh giá thành công");
            ToastMessage(1, "Cập nhật trạng thái đánh giá thành công");
            return Json(new
            {
                msg = "successful",
                content = "Cập nhật trạng thái đánh giá thành công"
            });
        }
        catch (Exception e)
        {
            ILoggingService.Error(this._iLogger, "Cập nhật trạng thái đánh giá lỗi", e.ToString(), e);
            return Json(new
            {
                msg = "fail",
                content = "Cập nhật trạng thái đánh giá lỗi: " + e
            });
        }
    }
    [HttpPost]
    [Authorize(Policy = "PermissionMVC")]
    public JsonResult Delete(int? id)
    {
        if (id == null)
        {
            ToastMessage(-1, "Không có dữ liệu đánh giá");
            return Json(new
            {
                msg = "fail",
                content = "Không có dữ liệu, không thể xóa"
            });
        }

        try
        {
            var comment = _iOrderCommentRepository.FindById(id.Value);
            if (comment != null)
            {
                //update product
                if (comment.Status ?? false)
                {
                    var product = _iProductRepository.FindById(comment.ProductId ?? 0);
                    product.Rate = Math.Max(0, (product.Rate == null ? 0 : product.Rate - comment.Rate ) ?? 0);
                    product.RateCount = Math.Max(0, (product.RateCount == null ? 0 : product.RateCount  - 1) ?? 0);
                    product.TotalComment =  Math.Max(0,(product.TotalComment == null ? 0 : product.TotalComment  - 1) ?? 0);
                    _iProductRepository.Update(product);
                }
                _iOrderCommentRepository.Delete(comment);
                ILoggingService.Infor(this._iLogger, "Xóa đánh giá thành công id:" + id, "UserId: " + UserInfo.UserId);
                ToastMessage(1, "Xóa đánh giá thành công");
                return Json(new
                {
                    msg = "successful",
                    content = "Xóa đánh giá  thành công"
                });
            }
            else
            {
                ILoggingService.Error(this._iLogger, "Xóa đánh giá lỗi" + "id:" + id, "UserId: " + UserInfo.UserId);
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
            ILoggingService.Error(this._iLogger, "Xóa đánh giá lỗi" + "id:" + id, "UserId: " + UserInfo.UserId, ex);
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
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "OrderComment@OrderCommentController@Delete")]
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
            int rs = this._iOrderCommentRepository.DeleteAllCommentRate(id);
            ToastMessage(1, $"Xóa thành công {rs} bản ghi");
            this._iLogger.LogInformation($"Xóa thành công {rs} đánh giá, UserId: " + UserInfo.UserId);
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
}