using Castle.Core.Internal;
using CMS.Areas.Admin.ViewModels.File;
using CMS.Controllers;
using CMS.Services.Files;
using CMS_Access.Repositories;
using CMS_EF.Models;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.StaticFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Text;
using CMS.Services.Token;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CMS.Filters;
using CMS.Services.Uris;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;

namespace CMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FileController : BaseController
    {
        private readonly IFileService _iFileService;
        private readonly IFilesRepository _iFilesRepository;
        private readonly ITokenService _iTokenService;
        private readonly ILogger<FileController> _iLogger;
        private readonly IUriService _iUriService;
        private readonly IPaginationFilter _iPaginationFilter;
        [Obsolete]
        private readonly IWebHostEnvironment _hostingEnvironment;

        [Obsolete]
        public FileController(
            ILogger<FileController> iLogger,
            IFileService iFileService, 
            IFilesRepository iFilesRepository,
            IUriService iUriService, 
            ITokenService iTokenService,
            IPaginationFilter iPaginationFilter,
            IWebHostEnvironment env
        ){
            this._iLogger = iLogger;
            this._hostingEnvironment = env;
            this._iFileService = iFileService;
            this._iFilesRepository = iFilesRepository;
            this._iTokenService = iTokenService;
            this._iPaginationFilter = iPaginationFilter;
            this._iUriService = iUriService;
        }

        // GET: File
        [Obsolete]
        public async Task<ActionResult> Index(string txtSearch, string startTime, string endTime, int? type, int pageindex = 1)
        {
            var query = _iFilesRepository.FindAll();
            if (!string.IsNullOrEmpty(txtSearch))
            {
                query = query.Where(x => EF.Functions.Like(x.Name, "%" + txtSearch.Trim() + "%"));
            }

            if (type.HasValue)
            {
                if (type.Value > -1)
                {
                    query = query.Where(x => x.Type == type.Value);
                }
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = DateTime.ParseExact(startTime, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture);
                query = query.Where(x => x.CreatedAt > start);
            }

            if (!string.IsNullOrEmpty(endTime))
            {
                var end = DateTime.ParseExact(endTime, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture).AddDays(1);
                query = query.Where(x => x.CreatedAt < end);
            }
            var data = await PagingList<Files>.CreateAsync(query.OrderByDescending(x => x.CreatedAt), PageSize, pageindex);

            data.RouteValue = new RouteValueDictionary
            {
                {"txtSearch", txtSearch},
                {"startTime", startTime},
                {"endTime", endTime},
                {"type", type}
            };

            IndexFileViewModel model = new IndexFileViewModel { ListData = data, iTokenService = this._iTokenService};
            
            return View(model);
        }


        [Consumes("multipart/form-data")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonLoad]
        public JsonResult UploadFile()
        {
            try
            {
                var a = HttpContext.Request.Form.Files;
                if (!HttpContext.Request.Form.Files.IsNullOrEmpty() && HttpContext.Request.Form.Files.Count > 0)
                {
                    var listMsg = new List<string>();
                    foreach (var item in HttpContext.Request.Form.Files)
                    {
                        if (FileExtensionContentTypeProvider.CheckAllowTypeFile(item.ContentType))
                        {
                            if (_iFileService.CheckSizeFile(item.Length, this.AppSetting.GetValue<int>(CmsConsts.MaxUploadSize)))
                            {
                                var rs = _iFileService.UploadFile(item, UserInfo.UserId);
                                if (rs.Result != null)
                                {
                                    listMsg.Insert(0, $"<p style='color: #18a689;'> File <span style='font-weight: bold;'>{item.FileName}</span> upload thành công </p>");
                                }
                                else
                                {
                                    listMsg.Add($"<p style='color: #ed5565;'> File <span style='font-weight: bold;'>{item.FileName}</span> upload không thành công </p>");
                                }
                            }
                            else
                            {
                                listMsg.Add($"<p style='color: #ed5565;'>Không thể upload File <span style='font-weight: bold;'>{item.FileName}</span> lớn hơn {this.AppSetting.GetValue<int>(CmsConsts.MaxUploadSize)} </p>");
                            }
                        }
                        else
                        {
                            listMsg.Add($"<p style='color: #ed5565;'> File <span style='font-weight: bold;'>{item.FileName}</span> không đúng định dạng được hỗ trợ </p>");
                        }
                    }
                    return Json(new
                    {
                        msg = "successful",
                        content = listMsg,
                        detail = ""
                    });
                }
                else
                {
                    return Json(new
                    {
                        msg = "fail",
                        content = "Không có file để upload",
                        detail = ""
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    msg = "fail",
                    content = "Không thể upload được file, liên hệ người quản trị",
                    detail = ex.Message
                });
            }
        }

        [NonLoad]
        [HttpGet]
        [Obsolete]
        public async Task<IActionResult> GetDownloadFile(string token, string url = null)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    this._iLogger.LogError("File không tồn tại");
                    ToastMessage(-1, $"fail download file");
                    return !string.IsNullOrEmpty(url) ? Redirect(url) : NotFound();
                }

                var input = this._iTokenService.DecodeJwtToken(token);
                if (input == null)
                {
                    return !string.IsNullOrEmpty(url) ? Redirect(url) : NotFound();
                }

                string pathFile = input["pathFile"].ToString()!;
                string fileName = input["fileName"].ToString();
                if (pathFile != null && pathFile.StartsWith("/"))
                {
                    pathFile = input["pathFile"].ToString()!.ReplaceFirst("/", "");
                }
                var fileName1 = System.IO.Path.GetFileName(Path.Combine(_hostingEnvironment.WebRootPath, pathFile!));
                var content = await System.IO.File.ReadAllBytesAsync(Path.Combine(_hostingEnvironment.WebRootPath, pathFile!));
                return File(content, "application/force-download", fileName1);

                //byte[] fileBytes = null;


                //using (var fs = new FileStream(Path.Combine(_hostingEnvironment.WebRootPath, pathFile!), FileMode.Open,
                //    FileAccess.Read))
                //{
                //    fileBytes = new byte[fs.Length];
                //    fs.Read(fileBytes, 0, (int) fs.Length);
                //}

                //return File(fileBytes, "application/force-download", fileName);
            }
            catch (DirectoryNotFoundException ex)
            {
                this._iLogger.LogError("fail download file:", ex);
                ToastMessage(-1, $"File không tồn tại trên hệ thống");
                return !string.IsNullOrEmpty(url) ? Redirect(url) : NotFound();
            }
            catch (FileNotFoundException ex)
            {
                this._iLogger.LogError("fail download file:", ex);
                ToastMessage(-1, $"File không tồn tại trên hệ thống");
                return !string.IsNullOrEmpty(url) ? Redirect(url) : NotFound();
            }
            catch (Exception ex)
            {
                this._iLogger.LogError("fail download file:", ex);
                ToastMessage(-1, $"Tải file không thành công {ex.Message.Replace("'","")}");
                return !string.IsNullOrEmpty(url) ? Redirect(url) : NotFound();
            }
        }

        [NonLoad]
        [HttpGet]
        [Obsolete]
        public async Task<IActionResult> GetDownloadFileAll(string linkFile, string url = null, string fileName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(linkFile))
                {
                    this._iLogger.LogError("File không tồn tại");
                    ToastMessage(-1, $"fail download file");
                    return !string.IsNullOrEmpty(url) ? Redirect(url) :  Redirect(Request.Headers["Referer"].ToString()); 
                }

                if (linkFile.StartsWith("/"))
                {
                    linkFile = linkFile.ReplaceFirst("/", "");
                }

                var fileName1 = System.IO.Path.GetFileName(Path.Combine(_hostingEnvironment.WebRootPath, linkFile!));
                var content = await System.IO.File.ReadAllBytesAsync(Path.Combine(_hostingEnvironment.WebRootPath, linkFile!));
                return File(content, "application/force-download", fileName == null ? fileName1 : fileName);

                //byte[] fileBytes = null;


                //using var fs = new FileStream(Path.Combine(_hostingEnvironment.WebRootPath, linkFile!), FileMode.Open,
                //    FileAccess.Read);
                //fileBytes = new byte[fs.Length];
                //fs.Read(fileBytes, 0, (int)fs.Length);

                //return File(fileBytes, "application/force-download", fileName == null ? fs.Name : fileName);
            }
            catch (DirectoryNotFoundException ex)
            {
                this._iLogger.LogError("fail download file:", ex);
                ToastMessage(-1, $"File không tồn tại trên hệ thống");
                return !string.IsNullOrEmpty(url) ? Redirect(url) : Redirect(Request.Headers["Referer"].ToString());
            }
            catch (FileNotFoundException ex)
            {
                this._iLogger.LogError("fail download file:", ex);
                ToastMessage(-1, $"File không tồn tại trên hệ thống");
                return !string.IsNullOrEmpty(url) ? Redirect(url) : Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                this._iLogger.LogError("fail download file:", ex);
                ToastMessage(-1, $"Tải file không thành công {ex.Message.Replace("'", "")}");
                return !string.IsNullOrEmpty(url) ? Redirect(url) : Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonLoad]
        public JsonResult UploadAvatar(IFormFile file)
        {
            try
            {
                if (file != null)
                {
                    if (file.ContentType.ToLower().StartsWith("image/"))
                    {
                        string url = _iFileService.UploadAvatar(file, UserInfo.UserId);
                        if (!string.IsNullOrEmpty(url))
                        {
                            return Json(new
                            {
                                msg = "successful",
                                content = "Upload ảnh đại diện thành công",
                                detail = url
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            msg = "fail",
                            content = "Không thể upload ảnh đại diện, vì không đúng định dạng ảnh",
                            detail = ""
                        });
                    }
                }
                return Json(new
                {
                    msg = "fail",
                    content = "Không thể upload ảnh đại diện",
                    detail = ""
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    msg = "fail",
                    content = "Không thể upload ảnh đại diện, liên hệ người quản trị",
                    detail = ex.ToString()
                });
            }
        }
        
        [Consumes("multipart/form-data")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonLoad]
        public JsonResult UploadAFile(IFormFile file)
        {
            try
            {
                if (FileExtensionContentTypeProvider.CheckAllowTypeFile(file.ContentType))
                {
                    if (_iFileService.CheckSizeFile(file.Length, this.AppSetting.GetValue<int>(CmsConsts.MaxUploadSize)))
                    {
                        var rs = _iFileService.UploadFile(file, UserInfo.UserId);
                        if (rs.Result != null)
                        {
                            return Json(new
                            {
                                msg = "successful",
                                content = new
                                {
                                    file = rs.Result
                                }
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                msg = "fail",
                                content = "Không thể upload được file, liên hệ người quản trị"
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            msg = "fail",
                            content = $"Không thể upload được file, file lớn hơn {this.AppSetting.GetValue<int>(CmsConsts.MaxUploadSize)}!"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        msg = "fail",
                        content = "Không thể upload được file, file không đúng định dạng được hỗ trợ!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    msg = "fail",
                    content = "Không thể upload được file, liên hệ người quản trị",
                    detail = ex.Message
                });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu");
                return Json(new
                {
                    msg = "fail",
                    content = "Nhóm quyền này không tồn tại trong hệ thống, không thể xóa"
                });
            }

            try
            {
                var result = _iFilesRepository.Delete((int)id);
                if (result)
                {
                    ToastMessage(1, "Xóa file thành công");
                    return Json(new
                    {
                        msg = "successful",
                        content = "Xóa file thành công"
                    });
                }
                else
                {
                    ToastMessage(-1, "Xóa file lỗi");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Không thể xóa file này, vui lòng liên hệ với người quản trị"
                    });
                }
            }
            catch (Exception)
            {
                ToastMessage(-1, "Xóa file lỗi");
                return Json(new
                {
                    msg = "fail",
                    content = "Không thể xóa file này, vui lòng liên hệ với người quản trị"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonLoad]
        public JsonResult DeleteAll(List<int> id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu file");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                int rs = this._iFilesRepository.DeleteAll(id);
                ToastMessage(1, $"Xóa thành công {rs} bản ghi");
                this._iLogger.LogInformation($"Xóa thành công {rs} file");
                return Json(new
                {
                    msg = "successful",
                    content = ""
                });
            }
            catch (Exception)
            {
                ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                this._iLogger.LogError($"Xóa dữ liệu lỗi, liên hệ người quản trị: id {id}");
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                });
            }
        }

        [HttpGet]
        [Obsolete]
        // [ValidateAntiForgeryToken]
        public JsonResult GetFilterFile()
        {
            IDictionary<string, string> listTypes = new Dictionary<string, string>();
            listTypes.Add("0","Khác");
            listTypes.Add("1","Hình ảnh");
            listTypes.Add("2","Video");
            listTypes.Add("3","Âm thanh");
            listTypes.Add("4","File PDF");
            listTypes.Add("5","File word");
            listTypes.Add("6","File excel");
            listTypes.Add("7","File powerpoint");
            listTypes.Add("8","File nén");
            
            List<string> listCreatedAt = this._iFilesRepository.GetListCreatedAt();
            FiltersApiModel res = new FiltersApiModel
            {
                ListTypes = listTypes,
                ListCreatedAt = listCreatedAt
            };
            return Json(new {
                succeeded = true,
                msg = "successful",
                content = res,
            });
        }

        [HttpGet]
        [Obsolete]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAllListFile(string txtSearch, string CreateAt, int? Type, int pageSize = 24, int pageNumber = 1)
        {
          
            this._iPaginationFilter.PageNumber = pageNumber;
            this._iPaginationFilter.PageSize = pageSize;

            var route = Request.Path.Value;

            var query = _iFilesRepository.FindAll();
            if (!string.IsNullOrEmpty(txtSearch))
            {
                query = query.Where(x => EF.Functions.Like(x.Name, "%" + txtSearch + "%"));
            }

            if (Type != null)
            {
                query = query.Where(x => x.Type == Type);
            }

            if (!string.IsNullOrEmpty(CreateAt))
            {
                DateTime start = DateTime.ParseExact(CreateAt, "M/yyyy", CultureInfo.InvariantCulture);

                int DaysInMonth = DateTime.DaysInMonth(start.Year, start.Month);

                DateTime end = new DateTime(start.Year, start.Month, DaysInMonth);

                query = query.Where(x => x.CreatedAt >= start && x.CreatedAt <= end);
            }

            var pagedData = await query.OrderByDescending(x => x.CreatedAt)
                    .Skip((this._iPaginationFilter.PageNumber - 1) * this._iPaginationFilter.PageSize)
                    .Take(this._iPaginationFilter.PageSize)
                    .ToListAsync();

            var totalRecords = _iFilesRepository.CountAsync().Result;

            var totalPages = ((double)totalRecords / (double)this._iPaginationFilter.PageSize);

            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            IPaginationFilter nextValidFilter = this._iPaginationFilter.GetNextPage();

            Uri nextPage = nextValidFilter.PageNumber >= 1 && nextValidFilter.PageNumber <= roundedTotalPages ? this._iUriService.GetPageUri(nextValidFilter, route) : null;

            PagingApiModel dataContent = new PagingApiModel(pagedData, this._iPaginationFilter, nextPage, "successful");

            return Json(dataContent); 
        }
    }
}