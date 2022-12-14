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
            _iLogger.LogError(e,$"Danh s??ch ????nh gi??");
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
            ILoggingService.Infor(this._iLogger, "C???p nh???t tr???ng th??i ????nh gi?? th??nh c??ng");
            ToastMessage(1, "C???p nh???t tr???ng th??i ????nh gi?? th??nh c??ng");
            return Json(new
            {
                msg = "successful",
                content = "C???p nh???t tr???ng th??i ????nh gi?? th??nh c??ng"
            });
        }
        catch (Exception e)
        {
            ILoggingService.Error(this._iLogger, "C???p nh???t tr???ng th??i ????nh gi?? l???i", e.ToString(), e);
            return Json(new
            {
                msg = "fail",
                content = "C???p nh???t tr???ng th??i ????nh gi?? l???i: " + e
            });
        }
    }
    [HttpPost]
    [Authorize(Policy = "PermissionMVC")]
    public JsonResult Delete(int? id)
    {
        if (id == null)
        {
            ToastMessage(-1, "Kh??ng c?? d??? li???u ????nh gi??");
            return Json(new
            {
                msg = "fail",
                content = "Kh??ng c?? d??? li???u, kh??ng th??? x??a"
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
                ILoggingService.Infor(this._iLogger, "X??a ????nh gi?? th??nh c??ng id:" + id, "UserId: " + UserInfo.UserId);
                ToastMessage(1, "X??a ????nh gi?? th??nh c??ng");
                return Json(new
                {
                    msg = "successful",
                    content = "X??a ????nh gi??  th??nh c??ng"
                });
            }
            else
            {
                ILoggingService.Error(this._iLogger, "X??a ????nh gi?? l???i" + "id:" + id, "UserId: " + UserInfo.UserId);
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
            ILoggingService.Error(this._iLogger, "X??a ????nh gi?? l???i" + "id:" + id, "UserId: " + UserInfo.UserId, ex);
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
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "OrderComment@OrderCommentController@Delete")]
    [NonLoad]
    public JsonResult DeleteAll(List<int> id)
    {
        if (id == null)
        {
            ToastMessage(-1, "Kh??ng c?? d??? li???u tin t???c");
            return Json(new
            {
                msg = "fail",
                content = "Kh??ng c?? d??? li???u, kh??ng th??? x??a"
            });
        }

        try
        {
            int rs = this._iOrderCommentRepository.DeleteAllCommentRate(id);
            ToastMessage(1, $"X??a th??nh c??ng {rs} b???n ghi");
            this._iLogger.LogInformation($"X??a th??nh c??ng {rs} ????nh gi??, UserId: " + UserInfo.UserId);
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
}