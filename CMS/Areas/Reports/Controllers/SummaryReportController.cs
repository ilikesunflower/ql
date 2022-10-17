using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ClosedXML.Report;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using CMS.Areas.Orders.Const;
using CMS.Areas.Reports.Models.SummaryReports;
using CMS.Areas.Reports.Services;
using CMS.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using MoreLinq;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Reports.Controllers;

[Area("Reports")]
[Obsolete]
public class SummaryReportController : BaseController
{
    private readonly ILogger<SummaryReportController> _iLogger;
    private readonly ISummaryReportService _summaryReportService;
    private readonly IWebHostEnvironment _iHostingEnvironment;

    public SummaryReportController(ILogger<SummaryReportController> iLogger, ISummaryReportService summaryReportService, IWebHostEnvironment iHostingEnvironment)
    {
        _iLogger = iLogger;
        _summaryReportService = summaryReportService;
        _iHostingEnvironment = iHostingEnvironment;
    }

    [Authorize(Policy = "PermissionMVC")]
    [HttpGet]
    public IActionResult Index(int isExport, string txtSearch, string startDate, string endDate,int? paymentStatus,int? status, int pageindex = 1)
    {
        try
        {
            DateTime now = DateTime.Now;
            DateTime lastMonth = DateTime.Now.AddMonths(-1);
            DateTime start = new DateTime(lastMonth.Year, lastMonth.Month, lastMonth.Day, 00, 00, 00);
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

            if (isExport == 1)
            {
                return RedirectToAction("Export",
                    new
                    {
                        startDate = start.ToString("dd/MM/yyyy"),
                        endDate = end.ToString("dd/MM/yyyy"),
                        txtSearch,
                        paymentStatus,
                        status
                    });
            }

            IQueryable<CMS_EF.Models.Orders.Orders> orders = _summaryReportService.GetListOrders(txtSearch,start, end,paymentStatus,status,false);
            var listData = PagingList.Create(orders.OrderByDescending(x => x.OrderAt), PageSize, pageindex);
            listData.RouteValue = new RouteValueDictionary()
            {
                {"txtSearch", txtSearch},
                {"startDate", start.ToString("dd/MM/yyyy")},
                {"endDate", end.ToString("dd/MM/yyyy")},
                {"paymentStatus", paymentStatus},
                {"status", status}
            };

            var model = new IndexViewModel
            {
                ListData = listData,
                OrderStatusPayments = OrderStatusPayment.ListOrderStatusPayment,
                ListStatus = OrderStatusConst.ListStatus.Where(x => x.Key != OrderStatusConst.StatusOrderCancel).ToDictionary(),
                IsDetailOrder = User.HasClaim(CmsClaimType.AreaControllerAction,
                    "Orders@OrderController@Details".ToUpper())
            };

            ILoggingService.Infor(_iLogger, "Xem báo cáo tổng hợp");

            return View(model);
        }
        catch (Exception e)
        {
            ILoggingService.Infor(_iLogger, "Xem báo cáo tổng hợp", "Lỗi: " + e.Message);
            return View(new IndexViewModel());
        }
    }

    [HttpGet]
    [NonLoad]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "Reports@SummaryReportController@Index")]
    public IActionResult Export( string txtSearch,string startDate, string endDate,int? paymentStatus,int? status)
    {
        try
        {
            DateTime? start = null;
            DateTime? end = null;
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

            var filePath = Path.Combine(_iHostingEnvironment.WebRootPath, "Templates/Excels/Reports",
                "SummaryReportTemplate.xlsx");
            var template = new XLTemplate(filePath);

            List<ExportExcelModel> orders = _summaryReportService.GetListOrdersExcel(txtSearch,start, end, paymentStatus, status,false);
            template.AddVariable("Title", "Báo cáo tổng hợp");
            template.AddVariable("ListData", orders);
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

            ILoggingService.Infor(_iLogger, "Xuất file báo cáo tổng hợp", "Thành công");
            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"bao_cao_tong_hop_tu_{startDate}_den_{endDate}.xlsx");
        }
        catch (Exception e)
        {
            ILoggingService.Error(_iLogger, "Xuất file báo cáo tổng hợp", "Lỗi" + e.Message);
            ToastMessage(-1, "Xuất file báo cáo tổng hợp lỗi");
            return RedirectToAction("Index", new {startDate, endDate});
        }
    }
    
    
}