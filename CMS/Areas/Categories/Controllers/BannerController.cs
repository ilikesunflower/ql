using Castle.Core.Internal;
using CMS.Areas.Categories.Models.Banner;
using CMS.Controllers;
using CMS.Models.ModelContainner;
using CMS_Access.Repositories.Categories;
using CMS_EF.Models.Categories;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using CMS.Areas.Categories.Const;

namespace CMS.Areas.Categories.Controllers
{

    [Area("Categories")]
    [Obsolete]
    public class BannerController : BaseController
    {
        private readonly ILogger _iLogger;
        private readonly IBannerRepository _iBannerRepository;
        public BannerController(ILogger<BannerController> iLogger, IBannerRepository iBannerRepository)
        {
            _iLogger = iLogger;
            _iBannerRepository = iBannerRepository;
        }
        
        [HttpGet]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Index(int txtSearch, int? status, int pageindex = 1)
        {
            var query = _iBannerRepository.FindAll();
            if (txtSearch != null && txtSearch != 0)
            {
                query = query.Where(x => x.Alias == BannerConst.GetNameListStatus(txtSearch));
            }
            if (status != null && status != 0)
            {
                switch (status)
                {
                    case 1:
                        query = query.Where(x => x.Status);
                        break;
                    case 2:
                        query = query.Where(x => !x.Status);
                        break;
                }
            }

            var listData = PagingList.Create(query.OrderByDescending(x => x.LastModifiedAt), PageSize, pageindex);
            listData.RouteValue = new RouteValueDictionary()
        {
            {"txtSearch", txtSearch},
            {"status", status}
        };
            ModelCollection model = new ModelCollection();
            model.AddModel("ListData", listData);
            model.AddModel("ListStatus",  BannerConst.ListStatus);
            model.AddModel("Page", pageindex);
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Create()
        {
            CreateModel model = new CreateModel();
            model.ListBanner = BannerConst.ListStatus;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Categories@BannerController@Create")]
        [NonLoad]
        public IActionResult Create(CreateModel createData)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    Banner banner = new Banner
                    {
                        Alias = createData.Alias.Trim(),
                        Link = createData.Link,
                        Images = createData.Images,
                        ImagesMobile = createData.ImagesMobile,
                        Status = createData.Status == 1,
                        Ord = createData.Ord ?? 0,
                        LastModifiedAt = DateTime.Now,
                        LastModifiedBy = UserInfo.UserId
                    };

                    var res = _iBannerRepository.Create(banner);

                    if (res != null)
                    {
                        ToastMessage(1, "Thêm mới banner thành công");
                        ILoggingService.Infor(_iLogger, "Thêm mới banner thành công", "id:" + res.Id);
                        return RedirectToAction(nameof(Details), new { id = res.Id });
                    }

                    ILoggingService.Error(_iLogger, "Thêm mới banner lỗi");
                }
                catch (Exception e)
                {
                    ILoggingService.Error(this._iLogger, "Thêm mới banner lỗi: " + e.Message);
                }
                ToastMessage(-1, "Thêm mới banner lỗi, liên hệ người quản trị");
            }
            createData.ListBanner = BannerConst.ListStatus;
            createData.Images = CmsFunction.IsValidImage(createData.Images) ? "" : createData.Images;
            return View(createData);
        }

        [HttpGet]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Details(int id)
        {
            Banner model = _iBannerRepository.FindById(id);
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "PermissionMVC")]
        public JsonResult Delete(int? id)
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
                var company = _iBannerRepository.FindById(id.Value);
                if (company != null)
                {
                    _iBannerRepository.Delete(company);
                    ILoggingService.Infor(this._iLogger, "Xóa banner thành công id:" + id, "UserId: " + UserInfo.UserId);
                    ToastMessage(1, "Xóa banner thành công");
                    return Json(new
                    {
                        msg = "successful",
                        content = "Xóa banner thành công"
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
                ILoggingService.Error(this._iLogger, "Xóa banner lỗi" + "id:" + id, "UserId: " + UserInfo.UserId, ex);
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
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Categories@BannerController@Delete")]
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
                int rs = this._iBannerRepository.DeleteAll(id);
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

            var banner = _iBannerRepository.FindById(id.Value);
            if (banner == null)
            {
                return NotFound();
            }
            EditModel model = new EditModel
            {
                Id= banner.Id,
                Alias = banner.Alias,
                Link = banner.Link,
                Images = banner.Images,
                ImagesMobile = banner.ImagesMobile,
                Status = banner.Status ? 1 : 0,
                Ord = banner.Ord,
                ListBanner = BannerConst.ListStatus

            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Categories@BannerController@Edit")]
        [NonLoad]
        public IActionResult Edit(EditModel EditData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var banner = _iBannerRepository.FindById(EditData.Id);
                    banner.Alias = EditData.Alias.Trim();
                    banner.Link = EditData.Link;
                    banner.Images = EditData.Images;
                    banner.Status = EditData.Status == 1;
                    banner.Ord = EditData.Ord ?? 0;
                    banner.ImagesMobile = EditData.ImagesMobile;
                    _iBannerRepository.Update(banner);
                    ILoggingService.Infor(this._iLogger, "Chỉnh sửa banner thành công", "id:" + EditData.Id);
                    ToastMessage(1, "Chỉnh sửa banner thành công");
                    return RedirectToAction(nameof(Details), new { EditData.Id });
                }
                else
                {
                    EditData.ListBanner = BannerConst.ListStatus;
                    return View(EditData);
                }

            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "Chỉnh sửa banner lỗi" + "id:" + EditData.Id, "UserId :" + UserInfo.UserId, e);
                ToastMessage(-1, "Lỗi không thể sửa banner này, Vui lòng liên hệ người quản trị");
            }
            EditData.Images = CmsFunction.IsValidImage(EditData.Images) ? "" : EditData.Images;
            return View(EditData);
        }

    }
}
