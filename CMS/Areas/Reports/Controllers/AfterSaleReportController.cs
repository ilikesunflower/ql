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
                    ToastMessage(-1, "Tên báo cáo đã tồn tại ! ");
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
                        ToastMessage(1, "Thêm mới báo cáo sau bán hàng thành công");
                        ILoggingService.Infor(_iLogger, "Thêm mới báo cáo sau bán hàng thành công", "id:" + rs.Id);
                        return RedirectToAction(nameof(Index));
                }
                ILoggingService.Error(_iLogger, "Thêm mới báo cáo sau bán hàng lỗi");
            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "Thêm mới báo cáo sau bán hàng lỗi: " + e.Message);
            }
            ToastMessage(-1, "Thêm mới báo cáo sau bán hàng lỗi, liên hệ người quản trị");
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
            ToastMessage(-1, "Không có dữ liệu báo cáo sau bán hàng");
            return Json(new
            {
                msg = "fail",
                content = "Không có dữ liệu, không thể xóa"
            });
        }

        try
        {
            var report = _iReportAfterSalesRepository.FindById(id.Value);
            _iReportAfterSalesRepository.Delete(report);
            
            ToastMessage(1, "Xóa dữ liệu báo cáo sau bán hàng thành công");
            ILoggingService.Infor(this._iLogger, "Xóa dữ báo cáo sau bán hàng thành công" + "id:" + id, "UserId :" + UserInfo.UserId);
            return Json(new
            {
                msg = "successful",
                content = "Xóa dữ liệu thành công"
            });
        }
        catch (Exception ex)
        {
            ToastMessage(-1, "Xóa dữ liệu lỗi");
            ILoggingService.Error(this._iLogger, "Xóa dữ liệu lỗi, liên hệ người quản trị" + "id:" + id, "UserId :" + UserInfo.UserId, ex);
            return Json(new
            {
                msg = "fail",
                content = "Không thể xóa dữ liệu, liên hệ người quản trị"
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
            ToastMessage(-1, "Không có dữ liệu báo cáo sau bán hàng");
            return Json(new
            {
                msg = "fail",
                content = "Không có dữ liệu, không thể xóa"
            });
        }

        try
        {
            int rs = this._iReportAfterSalesRepository.DeleteAll(id);
            ToastMessage(1, $"Xóa thành công {rs} bản ghi");
            ILoggingService.Infor(this._iLogger, "Xóa dữ liệu báo cáo sau bán hàng thành công" + "id:" + id, "UserId :" + UserInfo.UserId);
            return Json(new
            {
                msg = "successful",
                content = ""
            });
        }
        catch (Exception e)
        {
            ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
            ILoggingService.Error(this._iLogger, "Xóa dữ liệu lỗi, liên hệ người quản trị" + "id:" + id, "UserId :" + UserInfo.UserId, e);
            return Json(new
            {
                msg = "fail",
                content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
            });
        }
    }
}