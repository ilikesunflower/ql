using CMS.Areas.Admin.ViewModels.ApplicationController;
using CMS.Controllers;
using CMS_Access.Repositories;
using CMS_EF.DbContext;
using CMS_EF.Models.Identity;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Service;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ReflectionIT.Mvc.Paging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.Admin.Controllers
{
    //    Admin/ApplicationControllers
    [Area("Admin")]
    [NonLoad]
    public class ApplicationControllersController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IReflectionService iReflectionService;
        private readonly IApplicationActionRepository iApplicationActionRepository;
        private readonly IApplicationControllerRepository iApplicationControllerRepository;

        public ApplicationControllersController(ApplicationDbContext context,
            IReflectionService iReflectionService, IApplicationActionRepository iApplicationActionRepository,
            IApplicationControllerRepository iApplicationControllerRepository)
        {
            _context = context;
            this.iReflectionService = iReflectionService;
            this.iApplicationActionRepository = iApplicationActionRepository;
            this.iApplicationControllerRepository = iApplicationControllerRepository;
        }

        [Obsolete]
        public async Task<IActionResult> Index(string txtKeyword, int pageindex = 1)
        {
            var query = _context.ApplicationControllers.Where(x => x.Flag == 0).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(txtKeyword))
                query = query.Where(p => EF.Functions.Like(p.Name, "%" + txtKeyword.Trim() + "%") || p.Title.Contains(txtKeyword.Trim()));

            var model = await PagingList<ApplicationController>.CreateAsync(query.OrderByDescending(x => x.CreatedAt), PageSize, pageindex);

            model.RouteValue = new RouteValueDictionary
            {
                {"txtKeyword", txtKeyword}
            };
            var rs = new IndexControllerViewModel
            {
                Page = (pageindex - 1) * PageSize + 1,
                ListData = model,
                Configuration = Configuration
            };
            return View(rs);
        }

        public IActionResult Capnhat()
        {
            try
            {
                var listController =
                    iReflectionService.GetController(typeof(Program).Assembly, AppSetting.GetValue<string>(CmsConsts.NamespaceController));
                var controllerOld = iApplicationControllerRepository.GetAllController().ToList();
                for (var i = 0; i < listController.Count; i++)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(listController[i].CustomAttributes.First().ConstructorArguments.FirstOrDefault().Value?.ToString()) && !string.IsNullOrEmpty(listController[i].Name))
                        {
                            string name =
                                $"{listController[i].CustomAttributes.First().ConstructorArguments.FirstOrDefault().Value}@{listController[i].Name}";
                            var controller = controllerOld.FirstOrDefault(a => a.Name == name && a.Flag == 0);
                            if (controller == null)
                            {
                                var controllerInsert = new ApplicationController
                                {
                                    Name = this.IHtmlSanitizer.Sanitize(name),
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = UserInfo.UserId
                                };
                                _context.ApplicationControllers.Add(controllerInsert);
                                _context.SaveChanges();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                ToastMessage(1, "Cập nhật controller thành công");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ToastMessage(-1, "Cập nhật controller lỗi " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var applicationController = await _context.ApplicationControllers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationController == null) return NotFound();

            return View(applicationController);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var applicationController = await _context.ApplicationControllers.FindAsync(id);
            if (applicationController == null) return NotFound();
            return View(applicationController);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Title,Name,Description")] ApplicationController applicationController)
        {
            if (id != applicationController.Id) return NotFound();

            if (ModelState.IsValid)
                try
                {
                    var m =
                        _context.ApplicationControllers.FirstOrDefault(x =>
                            x.Flag == 0 && x.Id == applicationController.Id);
                    m.Description = applicationController.Description;
                    m.Title = applicationController.Title;
                    _context.Update(m);
                    await _context.SaveChangesAsync();
                    ToastMessage(1, "Sửa thông tin controller thành công");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationControllerExists(applicationController.Id)) return NotFound();
                    ToastMessage(-1, "Sửa thông tin controller lỗi");
                }

            return View(applicationController);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    ToastMessage(-1, "Không có dữ liệu");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Không có dữ liệu trong hệ thống, không thể xóa"
                    });
                }
                var rs = iApplicationControllerRepository.Delete(id);
                if (rs)
                {
                    ToastMessage(1, "Xóa dữ liệu thành công");
                    return Json(new
                    {
                        msg = "successful",
                        content = "Xóa dữ liệu thành công"
                    });
                }
                else
                {
                    ToastMessage(-1, "Xóa dữ liệu lỗi");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Không có dữ liệu trong hệ thống, không thể xóa"
                    });
                }
            }
            catch (Exception)
            {
                ToastMessage(-1, "Xóa dữ liệu lỗi");
                return Json(new
                {
                    msg = "fail",
                    content = "Không thể xóa dữ liệu, liên hệ người quản trị"
                });
            }
        }

        [Obsolete]
        public async Task<IActionResult> IndexAction(int? controllerId, string txtKeyword, int page = 1)
        {
            if (controllerId == null) return NotFound();
            var query = _context.ApplicationActions.Where(x => x.Flag == 0 && x.Controller.Id == controllerId)
                .AsNoTracking();
            if (!string.IsNullOrWhiteSpace(txtKeyword))
                query = query.Where(p => EF.Functions.Like(p.Name, "%" + txtKeyword + "%"));

            var model = await PagingList<ApplicationAction>.CreateAsync(query.OrderByDescending(x => x.CreatedAt), PageSize, page);
            model.RouteValue = new RouteValueDictionary
            {
                {"controllerId", controllerId},
                {"txtKeyword", txtKeyword}
            };
            var applicationController = _context.ApplicationControllers.FirstOrDefault(x => x.Id == (int)controllerId);
            if (applicationController == null)
            {
                return NotFound();
            }

            var rs = new IndexActionViewModel
            {
                ControllerId = (int)controllerId,
                ControllerName = string.IsNullOrEmpty(applicationController.Title)
                    ? applicationController.Name
                    : applicationController.Title,
                ListAction = model,
                Configuration = Configuration
            };
            return View(rs);
        }

        public IActionResult DetailsAction(int? id)
        {
            if (id == null) return NotFound();
            var action = iApplicationActionRepository.FindById((int)id);

            if (action == null) { return NotFound(); }
            return View(action);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteAction(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu về menu, không thể xóa"
                });
            }

            try
            {
                iApplicationActionRepository.Delete(id);
                ToastMessage(1, "Xóa dữ liệu thành công");
                return Json(new
                {
                    msg = "successful",
                    content = "Xóa menu thành công"
                });
            }
            catch (Exception e)
            {
                ToastMessage(-1, "Xóa dữ liệu lỗi " + e.Message);
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu về menu, không thể xóa"
                });
            }
        }


        private bool ApplicationControllerExists(int id)
        {
            return iApplicationControllerRepository.IsCheckById(id);
        }

        private bool ApplicationActionExists(int id)
        {
            return _context.ApplicationActions.Any(e => e.Id == id);
        }

        #region Edit Action

        public IActionResult EditAction(int? id)
        {
            if (id == null) return NotFound();
            var aspAction = iApplicationActionRepository.FindById(id.Value);
            if (aspAction == null) { return NotFound(); }
            return View(aspAction);
        }

        [HttpPost]
        public async Task<IActionResult> EditAction(int id,
            [Bind("Id,Title,Name,Description")] ApplicationAction applicationAction)
        {
            if (id != applicationAction.Id) return NotFound();

            if (ModelState.IsValid)
                try
                {
                    var a = iApplicationActionRepository.FindById(id);
                    a.Description = this.IHtmlSanitizer.Sanitize(applicationAction.Description);
                    a.Title = this.IHtmlSanitizer.Sanitize(applicationAction.Title);
                    _context.Update(a);
                    await _context.SaveChangesAsync();
                    ToastMessage(1, "Sửa Action trong controller thành công");
                    return RedirectToAction(nameof(IndexAction), new { controllerId = a.Controller.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationActionExists(applicationAction.Id)) return NotFound();
                    ToastMessage(-1, "Sửa action trong controller lỗi");
                }

            return View(applicationAction);
        }

        public IActionResult CapnhatAction(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var controller = iApplicationControllerRepository.GetControllerById((int)id);
                if (controller == null) return NotFound();

                var arrName = controller.Name.Split("@");

                var listController = iReflectionService
                    .GetController(typeof(Program).Assembly, AppSetting.GetValue<string>(CmsConsts.NamespaceController))
                    .FirstOrDefault(x => x.Name == arrName[1] && x.CustomAttributes.First().ConstructorArguments.FirstOrDefault().Value?.ToString() == arrName[0]);
                var listActionController = iReflectionService.GetActions(listController);
                var listActionControllerOld = iApplicationActionRepository.GetAllActions();
                if (listActionController.Count == 0)
                {
                    ToastMessage(1, "Cập nhật action trong controller thành công");
                    return RedirectToAction(nameof(IndexAction), new { controllerId = id });
                }
                foreach (var t in listActionController)
                {
                    try
                    {
                        var name = arrName[0] + "@" + listController?.Name + "@" + t.Name;
                        var actionCheck =
                            listActionControllerOld.FirstOrDefault(x => x.Name == name && x.Controller.Id == controller.Id);
                        if (actionCheck == null)
                        {
                            var action = new ApplicationAction
                            {
                                Name = name,
                                Controller = controller,
                                CreatedAt = DateTime.Now,
                                CreatedBy = UserInfo.UserId
                            };
                            _context.Add(action);
                            _context.SaveChanges();
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                ToastMessage(1, "Cập nhật action trong controller thành công");
                return RedirectToAction(nameof(IndexAction), new { controllerId = id });
            }
            catch (Exception ex)
            {
                ToastMessage(-1, "Cập nhật action trong controller lỗi " + ex.Message);
                return RedirectToAction(nameof(IndexAction), new { controllerId = id });
            }
        }

        #endregion
    }
}