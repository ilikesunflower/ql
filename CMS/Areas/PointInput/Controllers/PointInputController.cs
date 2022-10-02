using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using CMS_Access.Repositories.Customers;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using CMS.Areas.PointInput.Const;
using CMS.Areas.PointInput.Models.PointInputs;
using CMS.Areas.PointInput.Services;
using CMS.Controllers;
using CMS.Models;
using CMS.Services.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.PointInput.Controllers;

[Area("PointInput")]
[Obsolete]
public class PointInputController : BaseController
{
    private readonly ILogger<PointInputController> _iLogger;
    private readonly IHistoryFileChargePointRepository _historyFileChargePointRepository;
    private readonly IPointInputServices _pointInputServices;
    private readonly IFileService _iFileService;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ICustomerPointRepository _customerPointRepository;

    public PointInputController(IHistoryFileChargePointRepository historyFileChargePointRepository,
        IPointInputServices pointInputServices, ILogger<PointInputController> iLogger, IFileService iFileService, IWebHostEnvironment hostingEnvironment, ICustomerPointRepository customerPointRepository)
    {
        _historyFileChargePointRepository = historyFileChargePointRepository;
        _pointInputServices = pointInputServices;
        _iLogger = iLogger;
        _iFileService = iFileService;
        _hostingEnvironment = hostingEnvironment;
        _customerPointRepository = customerPointRepository;
    }


    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Index(string txtSearch, string startDate, string endDate, int pageindex = 1)
    {
        var query = _historyFileChargePointRepository.FindAll();
        if (!string.IsNullOrEmpty(txtSearch))
        {
            query = query.Where(x => EF.Functions.Like(x.Code, "%" + txtSearch.Trim() + "%"));
        }

        if (!string.IsNullOrEmpty(startDate))
        {
            var start = DateTime.ParseExact(startDate + " 00:00:00 AM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.CreatedAt >= start);
        }

        if (!endDate.IsNullOrEmpty())
        {
            var end = DateTime.ParseExact(endDate + " 11:59:59 PM", "dd/MM/yyyy hh:mm:ss tt",
                CultureInfo.InvariantCulture);
            query = query.Where(x => x.CreatedAt <= end);
        }

        var listData = PagingList.Create(query.OrderByDescending(x => x.CreatedAt), PageSize, pageindex);
        listData.RouteValue = new RouteValueDictionary()
        {
            {"txtSearch", txtSearch},
            {"startDate", startDate},
            {"endDate", endDate}
        };
        var model = new IndexViewModel
        {
            Title = "Danh sách lịch sử nhập điểm",
            ListData = listData,
            IsUploadFile = IsUploadFile()
        };
        return View(model);
    }

    
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult Details([FromRoute]int id,[FromQuery] int pageindex = 1)
    {
        var file = _historyFileChargePointRepository.FindById(id);
        if (file == null)
        {
            return NotFound();
        }

        var query = _customerPointRepository.FindAll()
            .Include( x => x.Customer )
            .Where(x => x.HistoryFileChargeFileId == file.Id);

        var listData = PagingList.Create(query.OrderByDescending(x => x.CreatedAt), PageSize, pageindex);
       
        var model = new DetailsPointViewModel
        {
            File = file,
            ListPoint = listData,
            IsSendNotification = IsSendNotification()
        };
        return View(model);
    }
    
    [HttpGet]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult UpFile()
    {
        var model = new UpFileViewModel();
        return View(model);
    }

    [HttpPost]
    [Authorize(Policy = "PermissionMVC")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpFile([FromForm] UpFileViewModel form)
    {
        if (!ModelState.IsValid) return View(form);
        try
        {
            var file = form.File;

            var dataFile = _pointInputServices.ReadDataFromExcelAndValidate(file);
            var pathFileRoot = await _iFileService.UploadFileImport(file, PointInputConst.PathSaveFile);

            dataFile.CreateBy = UserInfo.UserId;
            dataFile.LinkFile = pathFileRoot;
            _pointInputServices.ImportData(dataFile);
            ToastMessage(1, "Nhập dữ liệu thành công");
            ILoggingService.Infor(_iLogger, "UpFile", "Nhập dữ liệu thành công code = " + dataFile.Code);
        }
        catch (Exception e)
        {
            ToastMessage(-1, "Nhập dữ liệu lỗi, " + e.Message);
            ILoggingService.Error(_iLogger, "UpFile", "Nhập dữ liệu lỗi, " + e.Message);
        }

        return View(form);
    }
    
    [HttpGet]
    public IActionResult DownloadUploadFile(int id)
    {
        try
        {
            var file = _historyFileChargePointRepository.FindById(id);
            if (file == null)
            {
                return NotFound();
            }
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath,file.LinkFile );
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            ILoggingService.Infor(_iLogger, "DownloadUploadFile", "Xuất file thành công");
            return File(fileBytes, "application/force-download", file.FileName);

        }
        catch (Exception e)
        {
            ILoggingService.Error(_iLogger, "DownloadUploadFile", "Xuất file lỗi: " + e.Message, e);
            return NotFound();
        }
    }
    
    [NonLoad]
    [HttpGet]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "PointInput@PointInputController@UpFile")]
    public IActionResult DownloadTemplateFile()
    {
        try
        {
            string fileName = "mau_nhap_diem.xlsx";
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Templates/Excels/Point/Example", fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            ILoggingService.Infor(_iLogger, "DownloadTemplateFile", "Xuất file thành công");
            return File(fileBytes, "application/force-download", fileName);
        }
        catch (Exception e)
        {
            ILoggingService.Error(_iLogger, "DownloadTemplateFile", "Xuất file lỗi: " + e.Message, e);
            return NotFound();
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public JsonResult Delete(int id)
    {
        try
        {
            _pointInputServices.Delete(id);
            
            ToastMessage(1, "Xóa dữ liệu thành công");
            _iLogger.LogInformation("Xóa dữ liệu thành công");
            return Json(new
            {
                msg = "successful",
                content = "Xóa dữ liệu thành công"
            });
        }
        catch (Exception ex)
        {
            ToastMessage(-1, "Xóa dữ liệu lỗi");
            _iLogger.LogError(ex,"Xóa dữ liệu lỗi");
            return Json(new
            {
                msg = "fail",
                content = "Không thể xóa dữ liệu, liên hệ người quản trị"
            });
        }
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ClaimRequirement(CmsClaimType.AreaControllerAction, "PointInput@PointInputController@Delete")]
    [NonLoad]
    public JsonResult DeleteAll(List<int> id)
    {
        if (id == null)
        {
            ToastMessage(-1, "Không có dữ liệu nhóm quyền");
            return Json(new
            {
                msg = "fail",
                content = "Không có dữ liệu, không thể xóa"
            });
        }

        try
        {
            _pointInputServices.Delete(id);
            ToastMessage(1, "Xóa dữ liệu thành công");
            _iLogger.LogInformation("Xóa dữ liệu thành công");
            return Json(new
            {
                msg = "successful",
                content = "Xóa dữ liệu thành công"
            });
        }
        catch (Exception ex)
        {
            ToastMessage(-1, "Xóa dữ liệu lỗi");
            _iLogger.LogError(ex,"Xóa dữ liệu lỗi");
            return Json(new
            {
                msg = "fail",
                content = "Không thể xóa dữ liệu, liên hệ người quản trị"
            });
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "PermissionMVC")]
    public IActionResult SendNotification(int id)
    {
        try
        {
            _pointInputServices.SendNotification(id);
            ToastMessage(1, "Gửi thông báo thành công");
            _iLogger.LogInformation("Gửi thông báo thành công");
            return Ok(new OutputObject(200,"","").Show());
        }
        catch (Exception ex)
        {
            ToastMessage(-1, "Gửi thông báo lỗi");
            _iLogger.LogError(ex,"Gửi thông báo lỗi");
            return Ok(new OutputObject(400,"","").Show());
        }
    }
    private bool IsSendNotification()
    {
        IConfigurationSection claimType = Configuration.GetSection("ClaimType");
        return User.HasClaim(claimType.GetValue<string>(CmsClaimType.ControllerAction), "PointInput@PointInputController@SendNotification".ToUpper());
    }
    private bool IsUploadFile()
    {
        IConfigurationSection claimType = Configuration.GetSection("ClaimType");
        return User.HasClaim(claimType.GetValue<string>(CmsClaimType.ControllerAction), "PointInput@PointInputController@UpFile".ToUpper());
    }
}