using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Castle.Core.Internal;
using CMS.Areas.Reports.Const;
using CMS.Areas.Reports.Models.AfterSaleReport;
using CMS.Controllers;
using CMS.Services.Files;
using CMS_Access.Repositories.Reports;
using CMS_EF.Models.Reports;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Reports.Controllers;

[Area("Reports")]
[Obsolete]
public class AfterSaleReportController : BaseController
{
    // GET
    private readonly ILogger<AfterSaleReportController> _iLogger;
    private readonly IReportAfterSalesRepository _iReportAfterSalesRepository;
    private readonly IFileService _iFileService;

    public AfterSaleReportController(ILogger<AfterSaleReportController> iLogger, IReportAfterSalesRepository iReportAfterSalesRepository, IFileService iFileService)
    {
        _iLogger = iLogger;
        _iReportAfterSalesRepository = iReportAfterSalesRepository;
        _iFileService = iFileService;
    }
    
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Index(string txtSearch, int? typeDate, int pageindex = 1)
    {
        IndexViewModel model = new IndexViewModel();
        var query = _iReportAfterSalesRepository.FindAll();
        if (!txtSearch.IsNullOrEmpty())
        {
            query = query.Where(x => EF.Functions.Like(x.Name, "%" + txtSearch.Trim() + "%"));
        }

        if (typeDate.HasValue && typeDate > 0)
        {
            query = query.Where(x => x.Type == typeDate);
        }
        model.ListData = PagingList.Create(query.OrderByDescending(x => x.CreatedAt),PageSize, pageindex);
        model.ListData.RouteValue = new RouteValueDictionary()
        {
            {"txtSearch", txtSearch},
            {"typeDate", typeDate},
        };
        model.Page = pageindex;
        return View(model);
    }

    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Create()
    {
        DateTime t = DateTime.Now;
        CreateViewModel model = new CreateViewModel();
        model.Type = 1;
        model.Month = t.AddMonths(-1).ToString("MM/yyyy");
        model.Quater = CmsFunction.ConvertDateTimeToQuarterString(t);
        model.Year =  t.Year.ToString();
        return View(model);
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Reports@AfterSaleReportController@Create")]
    [NonLoad]
    public IActionResult Create(CreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var check = _iReportAfterSalesRepository.FindAll().Where(x => x.Name == model.Name.Trim())
                    .FirstOrDefault();
                if (check != null)
                {
                    ToastMessage(-1, "T??n b??o c??o ???? t???n t???i ! ");
                    return View(model);
                }
                ReportAfterSales reportAfterSales = new ReportAfterSales();
                reportAfterSales.Name = model.Name.Trim();
                reportAfterSales.Type = model.Type;
                if (model.Type == AfterSaleConst.month)
                {
                    DateTime timeQ = CmsFunction.ConvertStringToDateTime($"01/{model.Month}")!.Value;
                    reportAfterSales.Month = timeQ.Month;
                    reportAfterSales.Year = timeQ.Year;
                }else if (model.Type == AfterSaleConst.quarter)
                {
                    List<DateTime> timeQ =  CmsFunction.ParseQuarterToStartEndDate(model.Quater);
                    reportAfterSales.Quater = CmsFunction.ParseQuarterToInt(model.Quater);
                    reportAfterSales.Year = timeQ?[0].Year ?? 0;
                }
                else
                {
                    reportAfterSales.Year = Int32.Parse(model.Year);
                }
                reportAfterSales.CreatedAt = DateTime.Now;
                reportAfterSales.CreatedBy = UserInfo.UserId;
                if (model.File != null)
                {
                    var pathFileRoot =  _iFileService.UploadFileImport(model.File, AfterSaleConst.PathSaveFile);
                    reportAfterSales.LinkFile = pathFileRoot.Result;
                }
                var rs =  _iReportAfterSalesRepository.Create(reportAfterSales);
                if (rs != null)
                {
                        ToastMessage(1, "Th??m m???i b??o c??o sau b??n h??ng th??nh c??ng");
                        ILoggingService.Infor(_iLogger, "Th??m m???i b??o c??o sau b??n h??ng th??nh c??ng", "id:" + rs.Id);
                        return RedirectToAction(nameof(Index));
                }
                ILoggingService.Error(_iLogger, "Th??m m???i b??o c??o sau b??n h??ng l???i");
            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "Th??m m???i b??o c??o sau b??n h??ng l???i: " + e.Message);
            }
            ToastMessage(-1, "Th??m m???i b??o c??o sau b??n h??ng l???i, li??n h??? ng?????i qu???n tr???");
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "PermissionMVC")]
    public JsonResult Delete(int? id)
    {
        if (id == null)
        {
            ToastMessage(-1, "Kh??ng c?? d??? li???u b??o c??o sau b??n h??ng");
            return Json(new
            {
                msg = "fail",
                content = "Kh??ng c?? d??? li???u, kh??ng th??? x??a"
            });
        }

        try
        {
            var report = _iReportAfterSalesRepository.FindById(id.Value);
            _iReportAfterSalesRepository.Delete(report);
            
            ToastMessage(1, "X??a d??? li???u b??o c??o sau b??n h??ng th??nh c??ng");
            ILoggingService.Infor(this._iLogger, "X??a d??? b??o c??o sau b??n h??ng th??nh c??ng" + "id:" + id, "UserId :" + UserInfo.UserId);
            return Json(new
            {
                msg = "successful",
                content = "X??a d??? li???u th??nh c??ng"
            });
        }
        catch (Exception ex)
        {
            ToastMessage(-1, "X??a d??? li???u l???i");
            ILoggingService.Error(this._iLogger, "X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???" + "id:" + id, "UserId :" + UserInfo.UserId, ex);
            return Json(new
            {
                msg = "fail",
                content = "Kh??ng th??? x??a d??? li???u, li??n h??? ng?????i qu???n tr???"
            });
        }
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Reports@AfterSaleReportController@Delete")]
    [NonLoad]
    public JsonResult DeleteAll(List<int> id)
    {
        if (id == null)
        {
            ToastMessage(-1, "Kh??ng c?? d??? li???u b??o c??o sau b??n h??ng");
            return Json(new
            {
                msg = "fail",
                content = "Kh??ng c?? d??? li???u, kh??ng th??? x??a"
            });
        }

        try
        {
            int rs = this._iReportAfterSalesRepository.DeleteAll(id);
            ToastMessage(1, $"X??a th??nh c??ng {rs} b???n ghi");
            ILoggingService.Infor(this._iLogger, "X??a d??? li???u b??o c??o sau b??n h??ng th??nh c??ng" + "id:" + id, "UserId :" + UserInfo.UserId);
            return Json(new
            {
                msg = "successful",
                content = ""
            });
        }
        catch (Exception e)
        {
            ToastMessage(-1, "X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???");
            ILoggingService.Error(this._iLogger, "X??a d??? li???u l???i, li??n h??? ng?????i qu???n tr???" + "id:" + id, "UserId :" + UserInfo.UserId, e);
            return Json(new
            {
                msg = "fail",
                content = "L???i kh??ng th??? x??a b???n ghi n??y, vui l??ng nh???p li??n h??? ng?????i qu???n tr???"
            });
        }
    }
}