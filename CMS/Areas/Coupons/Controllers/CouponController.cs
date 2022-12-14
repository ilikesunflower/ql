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
            ILoggingService.Error(_iLogger, "Index file coupon l???i ",
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
                        ILoggingService.Error(_iLogger, "L??u file l???i ", "L???i ");
                        ToastMessage(-1, $"L??u file excel cho coupon l???i ");
                        return RedirectToAction(nameof(Import));
                    }

                    var file = form.File;
                    if (file is {Length: > 0})
                    {
                        var checkValidate = _iCouponService.CheckDataCoupon(file);
                        if (!checkValidate)
                        {
                            ILoggingService.Error(_iLogger, "L???i kh??ng c?? m?? phi???u y??u c???u ");
                            ToastMessage(-1, $"File excel thi???u m?? phi???u y??u c???u ! Vui l??ng ch???nh s???a ");
                            return RedirectToAction(nameof(Import));
                        }

                        var checkSame = _iCouponService.CheckSameCoupon(file);
                        if (!checkSame)
                        {
                            ILoggingService.Error(_iLogger, "L???i tr??ng m?? phi???u y??u c???u ");
                            ToastMessage(-1, $"File excel tr??ng m?? phi???u y??u c???u! Vui l??ng ch???nh s???a ");
                            return RedirectToAction(nameof(Import));
                        }
                    }
                    _iCouponService.SaveDataCoupon(file, pathFile, UserInfo.UserId);
                    ToastMessage(1, $"Nh???p d??? li???u t??? file excel cho coupon th??nh c??ng");
                    ILoggingService.Infor(_iLogger, "Nh???p d??? li???u t??? file excel cho coupon th??nh c??ng.",
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
                ILoggingService.Error(_iLogger, "Nh???p d??? li???u t??? file excel cho coupon th???t b???i ",
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
            return Ok(new OutputObject(200,"","G???i th??ng b??o coupon th??nh c??ng").Show());  
        }
        catch (Exception ex)
        {
            ILoggingService.Error(this._iLogger, $"G???i th??ng b??o coupon l???i : id {id} , UserId: {UserInfo.UserId}",
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
                ToastMessage(-1, "Kh??ng c?? d??? li???u coupon");
                return Json(new
                {
                    msg = "fail",
                    content = "Kh??ng c?? d??? li???u, kh??ng th??? x??a"
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
                        ILoggingService.Infor(this._iLogger, "X??a coupon th??nh c??ng id:" + id, "UserId: " + UserInfo.UserId);
                        ToastMessage(1, "X??a coupon th??nh c??ng");
                        return Json(new
                        {
                            msg = "successful",
                            content = "X??a coupon th??nh c??ng"
                        }); 
                    }
                    else
                    {
                        ToastMessage(-1, "X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???");
                        return Json(new
                        {
                            msg = "fail",
                            content = "X??a d??? li???u l???i"
                        });
                    }
             
                }
                else
                {
                    ILoggingService.Error(this._iLogger, "X??a coupon l???i" + "id:" + id, "UserId: " + UserInfo.UserId);
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
                ILoggingService.Error(this._iLogger, "X??a copoun l???i" + "id:" + id, "UserId: " + UserInfo.UserId, ex);
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
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Coupons@CouponController@Delete")]
        [NonLoad]
        public JsonResult DeleteAll(List<int> id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Kh??ng c?? d??? li???u coupon");
                return Json(new
                {
                    msg = "fail",
                    content = "Kh??ng c?? d??? li???u, kh??ng th??? x??a"
                });
            }

            try
            {
                bool rs = _iCouponService.DeleteAllCouponByIdFile(id);
                if (rs)
                {
                    ToastMessage(1, $"X??a th??nh c??ng  b???n ghi");
                    this._iLogger.LogInformation($"X??a th??nh c??ng coupon, UserId: " + UserInfo.UserId);
                    return Json(new
                    {
                        msg = "successful",
                        content = ""
                    });
                }
                else
                {
                    ToastMessage(-1, "X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???");
                    return Json(new
                    {
                        msg = "fail",
                        content = "L???i kh??ng th??? x??a b???n ghi n??y, vui l??ng nh???p li??n h??? ng?????i qu???n tr???"
                    });
                }
          
            }
            catch (Exception e)
            {
                ToastMessage(-1, "X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???");
                ILoggingService.Error(this._iLogger, $"X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???: id {id} , UserId: {UserInfo.UserId}",
                    "UserId :" + UserInfo.UserId, e);
                return Json(new
                {
                    msg = "fail",
                    content = "L???i kh??ng th??? x??a b???n ghi n??y, vui l??ng nh???p li??n h??? ng?????i qu???n tr???"
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