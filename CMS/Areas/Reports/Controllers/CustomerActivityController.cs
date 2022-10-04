using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ClosedXML.Report;
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

            IndexViewModelCustomerType customerType = _iCustomerActivityService.GetTypeCustomerActive(txtSearch,start,end,type);
            IQueryable<TrackingOfCustomer> numberOfCustomerGroups =  _iCustomerTrackingRepository.GetTypeCustomerActiveDetails(txtSearch, start,end, type);
            
            var listData = PagingList.Create(numberOfCustomerGroups.OrderByDescending(x => x.ActiveTime), PageSize, pageindex);
            listData.RouteValue = new RouteValueDictionary()
            {
                {"txtSearch", txtSearch},
                {"startDate", start.ToString("dd/MM/yyyy")},
                {"endDate", end.ToString("dd/MM/yyyy")},
                {"type", type}
            };
            model.CustomerType = customerType;
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
            IndexViewModelCustomerType customerType = _iCustomerActivityService.GetTypeCustomerActive(txtSearch,start,end,type);

            List<TrackingOfCustomer> details = _iCustomerActivityService.GetTypeCustomerActiveDetails(txtSearch,start,end,type);
            
            var filePath = Path.Combine(_iHostingEnvironment.WebRootPath, "Templates/Excels/Reports",
                "CustomerActivityTemplate.xlsx");
            var template = new XLTemplate(filePath);

            template.AddVariable("ListData", details);
            template.AddVariable("Org", customerType.Org);
            template.AddVariable("Staff", customerType.Staff);
            template.AddVariable("GA", customerType.GA);
            template.AddVariable("From", startDate);
            template.AddVariable("To", endDate);
            template.Generate();
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
