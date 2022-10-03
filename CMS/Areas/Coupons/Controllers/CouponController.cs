using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using CMS.Areas.Coupons.Models;
using CMS.Areas.Coupons.Services;
using CMS.Areas.Customer.Services;
using CMS.Controllers;
using CMS.Models;
using CMS.Models.ModelContainner;
using CMS.Services.Files;
using CMS.Services.Token;
using CMS_Access.Repositories.Customers;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Coupons.Controllers;

[Area("Coupons")]
[Obsolete]
public class CouponController : BaseController
{
    private readonly ILogger _iLogger;
    private readonly IFileService _iFileService;
    private readonly ICouponService _iCouponService;
    private readonly ITokenService _iTokenService;
    private readonly IHistoryFileCouponRepository _iHistoryFileCouponRepository;

    public CouponController(ILogger<CouponController> iLogger, IFileService iFileService, ICouponService iCouponService,
        IHistoryFileCouponRepository iHistoryFileCouponRepository, ITokenService iTokenService)
    {
        _iLogger = iLogger;
        _iFileService = iFileService;
        _iCouponService = iCouponService;
        _iHistoryFileCouponRepository = iHistoryFileCouponRepository;
        _iTokenService = iTokenService;
    }
    // GET
    [Authorize(Policy = "PermissionMVC")]

    public IActionResult Index(string txtSearch, string startDate, string endDate, int pageindex = 1)
    {
        try
        {
            var query = _iHistoryFileCouponRepository.FindAll();
            if (!txtSearch.IsNullOrEmpty())
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.FileName, "%" + txtSearch.Trim() + "%")
                    ||
                    EF.Functions.Like(x.OrgName, "%" + txtSearch.Trim() + "%")  
                    ||
                    EF.Functions.Like(x.Code, "%" + txtSearch.Trim() + "%")
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

            var listData = PagingList.Create(query.OrderByDescending(x => x.CreatedAt), PageSize, pageindex);
            listData.RouteValue = new RouteValueDictionary()
            {
                { "txtSearch", txtSearch },
                { "startDate", startDate },
                { "endDate", endDate },
            };
            IndexCouponModel model = new IndexCouponModel();
            model.ListData = listData;
            model.Page = pageindex;
            model.TokenService = _iTokenService;
            model.IsImport =  User.HasClaim(CmsClaimType.AreaControllerAction,
                "Coupons@CouponController@Import".ToUpper());
            return View(model);
        }
        catch (Exception e)
        {
            ILoggingService.Error(_iLogger, "Index file coupon lỗi ",
                "UserId: " + UserInfo.UserId, e);
            return NotFound();
        }
    }
    
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Import()
    {
        return View();
    }
    [NonLoad]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Consumes("multipart/form-data")]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Coupons@CouponController@Import")]
    public  IActionResult Import([FromForm] DataExportViewModel form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string pathFile =  _iFileService.SavingFile(form.File, "upload/coupon");
                    if (pathFile == null)
                    {
                        ILoggingService.Error(_iLogger, "Lưu file lỗi ", "Lỗi ");
                        ToastMessage(-1, $"Lưu file excel cho coupon lỗi ");
                        return RedirectToAction(nameof(Import));
                    }

                    var file = form.File;
                    if (file is {Length: > 0})
                    {
                        var checkValidate = _iCouponService.CheckDataCoupon(file);
                        if (!checkValidate)
                        {
                            ILoggingService.Error(_iLogger, "Lỗi không có mã phiếu yêu cầu ");
                            ToastMessage(-1, $"File excel thiếu mã phiếu yêu cầu ! Vui lòng chỉnh sửa ");
                            return RedirectToAction(nameof(Import));
                        }

                        var checkSame = _iCouponService.CheckSameCoupon(file);
                        if (!checkSame)
                        {
                            ILoggingService.Error(_iLogger, "Lỗi trùng mã phiếu yêu cầu ");
                            ToastMessage(-1, $"File excel trùng mã phiếu yêu cầu! Vui lòng chỉnh sửa ");
                            return RedirectToAction(nameof(Import));
                        }
                    }
                    _iCouponService.SaveDataCoupon(file, pathFile, UserInfo.UserId);
                    ToastMessage(1, $"Nhập dữ liệu từ file excel cho coupon thành công");
                    ILoggingService.Infor(_iLogger, "Nhập dữ liệu từ file excel cho coupon thành công.",
                        "File name:" );   
                }
                else
                {
                    ToastMessage(-1,ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage)
                        .Distinct().FirstOrDefault());
                }
            }
            catch (Exception e)
            {
                ToastMessage(-1, e.Message);
                ILoggingService.Error(_iLogger, "Nhập dữ liệu từ file excel cho coupon thất bại ",
                    "UserId: " + UserInfo.UserId, e);
            }

            return RedirectToAction(nameof(Index));
        }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult SendNotification(int id)
    {
        try
        {
            _iCouponService.SendNotificationHistoryFileCoupon(id);
            return Ok(new OutputObject(200,"","Gửi thông báo coupon thành công").Show());  
        }
        catch (Exception ex)
        {
            ILoggingService.Error(this._iLogger, $"Gửi thông báo coupon lỗi : id {id} , UserId: {UserInfo.UserId}",
                "UserId :" + UserInfo.UserId, ex);
            return Ok(new OutputObject(400,"","").Show());
         
        }
    }    
        [HttpPost]
        [Authorize(Policy = "PermissionMVC")]
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu coupon");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                var historyFileCoupon = _iHistoryFileCouponRepository.FindById(id.Value);
                if (historyFileCoupon != null)
                {
                    var rs = _iCouponService.DeleteCouponByIdFile(historyFileCoupon);
                    if (rs)
                    {
                        ILoggingService.Infor(this._iLogger, "Xóa coupon thành công id:" + id, "UserId: " + UserInfo.UserId);
                        ToastMessage(1, "Xóa coupon thành công");
                        return Json(new
                        {
                            msg = "successful",
                            content = "Xóa coupon thành công"
                        }); 
                    }
                    else
                    {
                        ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                        return Json(new
                        {
                            msg = "fail",
                            content = "Xóa dữ liệu lỗi"
                        });
                    }
             
                }
                else
                {
                    ILoggingService.Error(this._iLogger, "Xóa coupon lỗi" + "id:" + id, "UserId: " + UserInfo.UserId);
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
                ILoggingService.Error(this._iLogger, "Xóa copoun lỗi" + "id:" + id, "UserId: " + UserInfo.UserId, ex);
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
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Coupons@CouponController@Delete")]
        [NonLoad]
        public JsonResult DeleteAll(List<int> id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu coupon");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                bool rs = _iCouponService.DeleteAllCouponByIdFile(id);
                if (rs)
                {
                    ToastMessage(1, $"Xóa thành công  bản ghi");
                    this._iLogger.LogInformation($"Xóa thành công coupon, UserId: " + UserInfo.UserId);
                    return Json(new
                    {
                        msg = "successful",
                        content = ""
                    });
                }
                else
                {
                    ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                    });
                }
          
            }
            catch (Exception e)
            {
                ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                ILoggingService.Error(this._iLogger, $"Xóa dữ liệu lỗi, liên hệ người quản trị: id {id} , UserId: {UserInfo.UserId}",
                    "UserId :" + UserInfo.UserId, e);
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                });
            }
        }
        [HttpGet]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Details(int id,  int pageindex = 1)
        {
            try
            {
                DetailCouponModel model = new DetailCouponModel();
                var historyFile = _iHistoryFileCouponRepository.FindById(id);
                if (historyFile == null)
                {
                    return NotFound();
                }

                model.HistoryFile = historyFile;
                var customerCoupon = _iCouponService.GetListCustomerCoupons(id);
                model.ListCustomerCoupon = PagingList.Create(customerCoupon.OrderBy(x => x.EndTimeUse.Value),  PageSize, pageindex);
                model.Page = pageindex;
                model.IsSendNotification = User.HasClaim(CmsClaimType.AreaControllerAction,
                    "Coupons@CouponController@SendNotification".ToUpper()) && (historyFile.IsSentNotification ?? false);
                return View(model);

            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, $"Detail err : id {id} , UserId: {UserInfo.UserId}",
                    "UserId :" + UserInfo.UserId, e);
            }
            return View();

        }
        
        
     
    
}