using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ClosedXML.Report;
using CMS.Areas.Customer.Const;
using CMS.Areas.Customer.Models.Customer;
using CMS.Areas.Customer.Services;
using CMS.Controllers;
using CMS.Models;
using CMS_Access.Repositories.Customers;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Customer.Controllers;

[Area("Customer")]
[Obsolete("Obsolete")]
public class CustomerController : BaseController
{
    private readonly ILogger<CustomerController> _iLogger;
    private readonly ICustomerService _iCustomerService;
    private readonly ICustomerRepository _iCustomerRepository;
    private readonly ICustomerCouponRepository _iCustomerCouponRepository;
    private readonly ICustomerPointRepository _customerPointRepository;
    private readonly IWebHostEnvironment _iHostingEnvironment;
    public CustomerController(ILogger<CustomerController> iLogger, ICustomerService iCustomerService, ICustomerRepository iCustomerRepository, 
        IWebHostEnvironment iHostingEnvironment, ICustomerCouponRepository iCustomerCouponRepository, ICustomerPointRepository customerPointRepository)
    {
        _iLogger = iLogger;
        _iCustomerService = iCustomerService;
        _iCustomerRepository = iCustomerRepository;
        _iHostingEnvironment = iHostingEnvironment;
        _iCustomerCouponRepository = iCustomerCouponRepository;
        _customerPointRepository = customerPointRepository;
    }
    
    // GET
    public async Task<IActionResult> Index(string txtSearch, int? Type, int? status, int? TypeGroup, int pageindex = 1)
    {
        IndexViewModel rs = new IndexViewModel();
        var query = this._iCustomerService.GetAll();
        if (!string.IsNullOrEmpty(txtSearch))
        {
            query = query.Where(p => EF.Functions.Like(p.UserName, "%" + txtSearch.Trim() + "%") ||  EF.Functions.Like(p.FullName, "%" + txtSearch.Trim() + "%"));
        }

        if (Type.HasValue)
        {
            query = query.Where(p => p.Type == Type);
        }
        
        if (TypeGroup.HasValue)
        {
            query = query.Where(p => p.TypeGroup == TypeGroup);
        }
        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status);
        }
        var model = await PagingList<CMS_EF.Models.Customers.Customer>.CreateAsync(query.OrderByDescending(x => x.Id), PageSize, pageindex);
        model.RouteValue = new RouteValueDictionary()
        {
            {"txtSearch", txtSearch},
            {"Type", Type},
            {"TypeGroup", TypeGroup},
            {"status", status},
        };
        rs.ListData = model;
        rs.IsExportFile = User.HasClaim(CmsClaimType.AreaControllerAction,
                                              "Customer@CustomerController@Export".ToUpper());
        rs.IsImportFile = User.HasClaim(CmsClaimType.AreaControllerAction,
            "Customer@CustomerController@Import".ToUpper());
        return View(rs);
        }
    
        [HttpGet]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Create()
        {
            CreateViewModel model = new CreateViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Customer@CustomerController@Create")]
        [NonLoad]
        public IActionResult Create(CreateViewModel createData)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var rs = this._iCustomerService.Create(createData);
                    if (rs != null)
                    {
                        ToastMessage(1, "Thêm mới tài khoản khách hàng thành công");
                        ILoggingService.Infor(_iLogger, "Thêm mới banner thành công", "id:" + rs.Id);
                        return RedirectToAction(nameof(Details), new { id = rs.Id });
                    }

                    ILoggingService.Error(_iLogger, "Thêm mới tài khoản khách hàng lỗi");
                }
                catch (Exception e)
                {
                    ILoggingService.Error(this._iLogger, "Thêm mới tài khoản khách hàng lỗi: " + e.Message);
                }
                ToastMessage(-1, "Thêm mới tài khoản khách hàng lỗi, liên hệ người quản trị");
            }
            return View(createData);
        }

        [HttpGet]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Details(int id)
        {
            DetailsViewModel rs = new DetailsViewModel();
            CMS_EF.Models.Customers.Customer model = _iCustomerRepository.FindById(id);
            var customerCoupon = _iCustomerCouponRepository.GetAllNoDateByCustomerId(id);
            rs.Customer = model;
            rs.IsResetPass =
                User.HasClaim(CmsClaimType.AreaControllerAction,
                    "Customer@CustomerController@ResetPassWord".ToUpper()) && (rs.Customer.Type == 2);
            return View(rs);
        }

        [HttpGet]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Customer@CustomerController@Details")]
        public IActionResult GetCouponCustomer(int idCustomer, int page)
        {
            var customerCoupons = _iCustomerCouponRepository.GetAllNoDateByCustomerId(idCustomer);
            var model =  PagingList.Create(customerCoupons.OrderByDescending(x => x.EndTimeUse), 20, page);
            return Json(new
            {
                code = 200,
                msg = "successful",
                content = model,
                model.PageCount,
                model.PageIndex,
            });
        }
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult ResetPassWord(int id)
        {
            try
            {
               var c = this._iCustomerRepository.FindById(id);
               if (c != null)
               {
                   var rs = this._iCustomerService.ResetPassWord(id);
                   if (rs)
                   {
                       ToastMessage(1, $"Reset mật khẩu cho tài khoản khách hàng {c.UserName} thành công");
                       return Ok(new OutputObject(200,"",$"Reset mật khẩu cho tài khoản khách hàng {c.UserName} thành công").Show());
                   }
                   else
                   {
                       return Ok(new OutputObject(400,"",$"Reset mật khẩu cho tài khoản khách hàng {c.UserName} lỗi").Show());
                   }
               }
            }
            catch (Exception ex)
            {
                // ignored
                this._iLogger.LogError(ex,$"Reset mật khẩu cho tài khoản khách hàng {id} lỗi");
            }
            return Ok(new OutputObject(400,"",$"Reset mật khẩu cho tài khoản khách hàng lỗi").Show());
        }
        
        [HttpPost]
        [Authorize(Policy = "PermissionMVC")]
        public ActionResult Import([FromForm] ImportCustomerViewModel form)
        {
            try
            {
            if (ModelState.IsValid)
            {
                IFormFile file = form.File;
                var workbook = new XLWorkbook(file.OpenReadStream());
                IXLWorksheet ws = workbook.Worksheet(1);
                IXLRange range = ws.RangeUsed();
                int rowCount = range.RowCount();
                List<CMS_EF.Models.Customers.Customer> listCUpdate = new List<CMS_EF.Models.Customers.Customer>();
                List<CreateViewModel> listCInsert = new List<CreateViewModel>();
                for (int i = 3; i <= rowCount; i++)
                {
                    IXLCell celUserName = range.Cell(i, 2);
                    IXLCell celFullName = range.Cell(i, 3);
                    IXLCell celEmail = range.Cell(i, 4);
                    IXLCell celPhone = range.Cell(i, 5);
                    IXLCell celOrg = range.Cell(i, 6);
                    CreateViewModel rs = new CreateViewModel()
                    {
                        UserName = celUserName.GetString(),
                        FullName = celFullName.GetString(),
                        Email = celEmail.GetString(),
                        Org = celOrg.GetString(),
                        Phone = celPhone.GetString(),
                    };
                    
                    if (!string.IsNullOrEmpty(rs.UserName) && !string.IsNullOrEmpty(rs.FullName) && !string.IsNullOrEmpty(rs.Email) 
                        && !CmsFunction.IsHtml(rs.UserName) && !CmsFunction.IsHtml(rs.FullName) && !CmsFunction.IsHtml(rs.Email)
                       && !CmsFunction.IsHtml(rs.Phone) && !CmsFunction.IsHtml(rs.Detail) && CmsFunction.IsValidUserName(rs.UserName))
                    {
                        var c =  this._iCustomerRepository.FindByUserName(rs.UserName.Trim());
                        if (c != null)
                        {
                            c.Org = rs.Org;
                            c.Email = rs.Email;
                            c.Phone = rs.Phone;
                            c.FullName = rs.FullName;
                            listCUpdate.Add(c);
                        }
                        else
                        {
                            listCInsert.Add(rs);
                        }
                    }
                }

                int count = 0;
                if (listCUpdate.Count > 0)
                {
                    this._iCustomerRepository.BulkUpdate(listCUpdate);
                }

                if (listCInsert.Count > 0)
                {
                    foreach (var item in listCInsert)
                    {
                        var r = this._iCustomerService.Create(item);
                        if (r != null)
                        {
                            count++;
                        }
                    }
                }

                count += listCUpdate.Count;
                ToastMessage(1,$"Import thành công {count} tài khoản khách hàng");
                return Ok(new OutputObject(200,"",$"Import thành công {count} tài khoản khách hàng").Show());
            }
            return Ok(new OutputObject(400,"",$"Import file khách hàng lỗi").Show());
            }
            catch (Exception ex)
            {
                this._iLogger.LogError(ex,"Import file khách hàng lỗi");
                return Ok(new OutputObject(400,"",$"Import file khách hàng lỗi").Show());
            }
        }
        
        [HttpGet]
        [Authorize(Policy = "PermissionMVC")]
        public ActionResult Export(Dictionary<string, string> data)
        {
            try
            {
                var list = this._iCustomerRepository.FindAll();
                if (data.ContainsKey("txtSearch") && !string.IsNullOrEmpty(data["txtSearch"]))
                    list = list.Where(p => EF.Functions.Like(p.UserName, "%" + data["txtSearch"].Trim() + "%") 
                                           || EF.Functions.Like(p.FullName, "%" + data["txtSearch"].Trim() + "%"));
                if (data.ContainsKey("TypeGroup") && !string.IsNullOrEmpty(data["TypeGroup"]) )
                {
                    int? TypeGroup = CmsFunction.ConvertToInt($"{data["TypeGroup"]}");
                    if (TypeGroup.HasValue)
                    {
                        list = list.Where(x => x.TypeGroup == TypeGroup);
                    }
                }
                if (data.ContainsKey("Type") && !string.IsNullOrEmpty(data["Type"]) )
                {
                    int? Type = CmsFunction.ConvertToInt($"{data["Type"]}");
                    if (Type.HasValue)
                    {
                        list = list.Where(x => x.Type == Type);
                    }
                }
                if (data.ContainsKey("status") && !string.IsNullOrEmpty(data["status"]) )
                {
                    int? status = CmsFunction.ConvertToInt($"{data["status"]}");
                    if (status.HasValue)
                    {
                        list = list.Where(x => x.Status == status);
                    }
                }
                
                
                var filePath = Path.Combine(_iHostingEnvironment.WebRootPath, "Templates/Excels/Customer",
                    "CustomerTemplate.xlsx");
                var template = new XLTemplate(filePath);

                template.AddVariable("ListData", list.OrderByDescending(x=>x.Id).ToList());
                template.Generate();
                byte[] excelFile;
                using (MemoryStream ms = new MemoryStream())
                {
                    template.SaveAs(ms);
                    ms.Position = 0;
                    excelFile = ms.ToArray();
                }

                ILoggingService.Infor(_iLogger, "Xuất file thông tin khách hàng thành công", "UserId" + UserInfo.UserId);
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"danh_sach_khach_hang.xlsx");
            }
            catch (Exception e)
            {
                ILoggingService.Error(_iLogger, "Xuất file danh sách khách hàng lỗi ", "UserId " + UserInfo.UserId, e);
            }

            ToastMessage(-1, "Xuất file lỗi");
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        [Authorize(Policy = "PermissionMVC")]
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu khách hàng");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                var data = this._iCustomerRepository.FindById(id.Value);
                if (data != null)
                {
                    this._iCustomerRepository.Delete(data);
                    ILoggingService.Infor(this._iLogger, "Xóa khách hàng thành công id:" + id, "UserId: " + UserInfo.UserId);
                    ToastMessage(1, "Xóa khách hàng thành công");
                    return Json(new
                    {
                        msg = "successful",
                        content = "Xóa khách hàng thành công"
                    });
                }
                else
                {
                    ILoggingService.Error(this._iLogger, "Xóa banner lỗi" + "id:" + id, "UserId: " + UserInfo.UserId);
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
                ILoggingService.Error(this._iLogger, "Xóa khách hàng lỗi" + "id:" + id, "UserId: " + UserInfo.UserId, ex);
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
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Customer@CustomerController@Delete")]
        [NonLoad]
        public JsonResult DeleteAll(List<int> id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu banner");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                int rs = this._iCustomerRepository.DeleteAll(id);
                ToastMessage(1, $"Xóa thành công {rs} bản ghi");
                this._iLogger.LogInformation($"Xóa thành công {rs} banner, UserId: " + UserInfo.UserId);
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

        [HttpGet]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var data = this._iCustomerRepository.FindById(id.Value);
            if (data == null)
            {
                return NotFound();
            }
            EditViewModel rs = new EditViewModel
            {
                UserName = data.UserName,
                Detail = data.Detail,
                Email = data.Email,
                FullName = data.FullName,
                Phone = data.Phone,
                Type = data.Type ?? 0,
                Status = data.Status ?? 0,
                TypeGroup = data.TypeGroup ?? 0,
                Org = data.Org
            };
            return View(rs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Customer@CustomerController@Edit")]
        [NonLoad]
        public IActionResult Edit(int? id,EditViewModel input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rs = this._iCustomerRepository.FindById(id.Value);
                    if (rs != null)
                    {
                        rs.Email = input.Email;
                        rs.Detail = input.Detail;
                        rs.Phone = input.Phone;
                        rs.TypeGroup = input.TypeGroup;
                        rs.FullName = input.FullName;
                        if (rs.Type == CustomerConst.TypeOrgPru)
                        {
                            rs.Org = input.Org;
                            rs.Status = input.Status;
                        }
                        this._iCustomerRepository.Update(rs);    
                        ILoggingService.Infor(this._iLogger, "Chỉnh sửa thông tin khách hàng thành công", $"id: {id}");
                        ToastMessage(1, "Chỉnh sửa thông tin khách hàng thành công");
                        return RedirectToAction(nameof(Details), new { id = rs.Id });
                    }
                    else
                    {
                        ILoggingService.Infor(this._iLogger, $"Chỉnh sửa thông tin khách hàng lỗi", $"id: {id}");
                        ToastMessage(-1, "Chỉnh sửa thông tin khách hàng lỗi");
                    }
                }
                else
                {
                    return View(input);
                }

            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "Chỉnh sửa thông tin khách hàng lỗi" + "id:" , "UserId :" + UserInfo.UserId, e);
                ToastMessage(-1, "Lỗi không thể sửa thông tin khách hàng này, Vui lòng liên hệ người quản trị");
            }
            return View(input);
        }


        [HttpGet("/Customer/Customer/{id:int}/Point")]
        public async Task<IActionResult> Point(int id,[FromQuery] int pageindex = 1)
        {
            try
            {
                var query = _customerPointRepository.FindAll()
                    .Where(x => x.Flag == 0 && x.CustomerId == id);
                
                
                var model = await PagingList<CMS_EF.Models.Customers.CustomerPoint>
                    .CreateAsync(query.OrderByDescending(x => x.EndTime), PageSize, pageindex);

                return Json(new
                {
                    msg = "successful",
                    content = "Lấy điểm thành công",
                    data = new
                    {
                        content = model.ToList(),
                        pageCount = model.PageCount,
                        perPage = PageSize,
                        currentPage = model.PageIndex
                    }
                }); 
                
            }
            catch (Exception e)
            {
                _iLogger.LogInformation("Lấy point không thành công" + e.Message);
                return Json(new
                {
                    msg = "failure",
                    content = "Lấy điểm thất bại"
                }); 
            }
        }
}