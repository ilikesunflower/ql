using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using ClosedXML.Report;
using CMS.Areas.Reports.Const;
using CMS_Access.Repositories.Customers;
using CMS.Areas.Reports.Models.CustomerActivity;
using CMS.Areas.Reports.Models.SummaryReports;
using CMS.Areas.Reports.Services;
using CMS.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;
using IndexViewModel = CMS.Areas.Reports.Models.CustomerActivity.IndexViewModel;

namespace CMS.Areas.Reports.Controllers;

[Area("Reports")]
[Obsolete]
public class CustomerActivityController : BaseController
{
    private readonly ILogger<CustomerActivityController> _iLogger;
    private readonly IWebHostEnvironment _iHostingEnvironment;
    private readonly ICustomerActivityService _iCustomerActivityService;
    private readonly ICustomerTrackingRepository _iCustomerTrackingRepository;
    public CustomerActivityController(ILogger<CustomerActivityController> iLogger,
        IWebHostEnvironment iHostingEnvironment, ICustomerActivityService iCustomerActivityService, ICustomerTrackingRepository iCustomerTrackingRepository)
    {
        _iLogger = iLogger;
        _iHostingEnvironment = iHostingEnvironment;
        _iCustomerActivityService = iCustomerActivityService;
        _iCustomerTrackingRepository = iCustomerTrackingRepository;
    }

    [Authorize(Policy = "PermissionMVC")]
    [HttpGet]
    public IActionResult Index(int isExport, string txtSearch, string startDate, string endDate, int? type, int pageindex = 1)
    {
        if (isExport == 1)
        {
            return RedirectToAction("Export", new
            {
                startDate,
                endDate,
                txtSearch,
                type
            });
        }
        try
        {
            IndexViewModel model = new IndexViewModel();

            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year, now.Month, now.Day, 00, 00, 00);
            DateTime end = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            if (!string.IsNullOrEmpty(startDate))
            {
                start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt",
                    CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt",
                    CultureInfo.InvariantCulture);
            }

            List<IndexCustomerType> customerType = _iCustomerActivityService.GetTypeCustomerActive(txtSearch,start,end,type);
            IQueryable<TrackingOfCustomer> numberOfCustomerGroups =  _iCustomerTrackingRepository.GetTypeCustomerActiveDetails(txtSearch, start,end, type);
            
            var listData = PagingList.Create(numberOfCustomerGroups.OrderByDescending(x => x.ActiveTime), PageSize, pageindex);
            listData.RouteValue = new RouteValueDictionary()
            {
                {"txtSearch", txtSearch},
                {"startDate", start.ToString("dd/MM/yyyy")},
                {"endDate", end.ToString("dd/MM/yyyy")},
                {"type", type}
            };
            model.CustomerTypeList = customerType;
            model.Customers = listData;
            
            return View(model);
        }
        catch (Exception e)
        {
            ILoggingService.Infor(_iLogger, "Xem báo cáo hoạt động nguười dùng", "Lỗi: " + e.Message);
            return BadRequest();
        }
    }

    [Authorize(Policy = "PermissionMVC")]
    [HttpGet]
    public IActionResult Export(string txtSearch, string startDate, string endDate, int? type, int pageindex = 1)
    {
        try
        {
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year, now.Month, now.Day, 00, 00, 00);
            DateTime end = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            if (!string.IsNullOrEmpty(startDate))
            {
                start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt",
                    CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt",
                    CultureInfo.InvariantCulture);
            }
            List<IndexCustomerType> customerType = _iCustomerActivityService.GetTypeCustomerActive(txtSearch,start,end,type);

            List<TrackingOfCustomer> details = _iCustomerActivityService.GetTypeCustomerActiveDetails(txtSearch,start,end,type);
            
            var filePath = Path.Combine(_iHostingEnvironment.WebRootPath, "Templates/Excels/Reports",
                "CustomerActivityTemplate.xlsx");
            var template = new XLTemplate(filePath);

            template.AddVariable("ListCustomerType", customerType);
            template.AddVariable("From", startDate);
            template.AddVariable("To", endDate);
            template.Generate();
            var wsh = template.Workbook.Worksheets.FirstOrDefault();
            IXLRange w = wsh!.Range(6 + customerType.Count, 2, 6 + customerType.Count, 6);
            ReportConst.MergeStyleExcel(w);
            ReportConst.SetExcelRangeBgColor(w);
            w.SetValue("Chi tiết khách hàng");
            
            
            ReportConst.SetTextTitle(wsh!.Cell(7 + customerType.Count , 2),   "STT");
            ReportConst.SetTextTitle(wsh!.Cell(7 + customerType.Count , 3),   "Tên khách hàng");
            ReportConst.SetTextTitle(wsh!.Cell(7 + customerType.Count , 4),   "ID Khách hàng");
            ReportConst.SetTextTitle(wsh!.Cell(7 + customerType.Count , 5),   "Loại khách hàng");
            ReportConst.SetTextTitle(wsh!.Cell(7 + customerType.Count , 6),   "Thời gian hoạt động");
            wsh.Column("F").Width  = 20;
            int index = 1;
            foreach (var item in details)
            {
                ReportConst.SetText(wsh!.Cell(7  + customerType.Count + index, 2),   index.ToString());
                ReportConst.SetText(wsh!.Cell(7  + customerType.Count + index, 3),   item.FullName ?? "");
                ReportConst.SetText(wsh!.Cell(7  + customerType.Count + index, 4),   item.Username ?? "");
                ReportConst.SetText(wsh!.Cell(7  + customerType.Count + index, 5),   item.Org ?? "");
                ReportConst.SetText(wsh!.Cell(7  + customerType.Count + index, 6),   item.ActiveTime.HasValue ? item.ActiveTime.Value.ToString("dd/MM/yyyy HH:mm") : ""  );
                index++;
            }
            byte[] excelFile;
            using (MemoryStream ms = new MemoryStream())
            {
                template.SaveAs(ms);
                ms.Position = 0;
                excelFile = ms.ToArray();
            }

            ILoggingService.Infor(_iLogger, "Xuất file Báo cáo hoạt động người dùng", "Thành công");
            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"bao_cao_hoat_dong_khach_hang_tu_{startDate}_den_{endDate}.xlsx");
        }
        catch (Exception e)
        {
            ILoggingService.Infor(_iLogger, "Xuất file Báo cáo hoạt động người dùng", "Lỗi: " + e.Message);
            return BadRequest();
        }
    }
    
    [Authorize(Policy = "PermissionMVC")]
    [HttpGet]
    public JsonResult WidgetDashboard(string startDate, string endDate)
    {
        try
        {
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year, now.Month, now.Day, 00, 00, 00);
            DateTime end = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            if (!string.IsNullOrEmpty(startDate))
            {
                start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt",
                    CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt",
                    CultureInfo.InvariantCulture);
            }
            List<IndexViewModelCustomerTypeChart> customerType = _iCustomerActivityService.GetTypeCustomerActiveChart(start,end);
      
            return Json(new
            {
                msg = "successful",
                content = customerType
            });
        }
        catch (Exception ex)
        {
            this._iLogger.LogError(ex,"");
            return Json(new
            {
                msg = "fail",
                content = ""
            });
        }
    }
}
