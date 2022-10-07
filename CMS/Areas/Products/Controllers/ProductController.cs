using CMS.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CMS.Areas.Categories.Const;
using CMS.Areas.Orders.Const;
using CMS.Areas.Products.Const;
using CMS_EF.Models.Products;
using CMS.Areas.Products.Models.Product;
using CMS.Areas.Products.Services;
using CMS.Models;
using CMS.Models.ModelContainner;
using CMS.Services.Files;
using CMS.Services.Loggings;
using CMS_Access.Repositories.Categories;
using CMS_Access.Repositories.Products;
using CMS_EF.DbContext;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Extensions.HtmlAgilityPack;
using CMS_Lib.Util;
using CMS.Config.Consts;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Novacode;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Products.Controllers
{
    [Area("Products")]
    [Obsolete]
    public class ProductController : BaseController
    {
        private readonly ILogger _iLogger;
        private readonly ILoggingService _iLoggingService;
        private readonly IProductRepository _iProductRepository;
        private readonly IProductPurposeRepository _iProductPurposeRepository;
        private readonly IProductPropertiesRepository _iProductPropertiesRepository;
        private readonly IProductCategoryRepository _iProductCategoryRepository;
        private readonly IProductCategoryProductRepository _iProductCategoryProductRepository;
        private readonly IProductImageRepository _iProductImageRepository;
        private readonly IProductSimilarRepository _iProductSimilarRepository;
        private readonly IProductSimilarPropertyRepository _iProductSimilarPropertyRepository;
        private readonly IProductPropertiesValueRepository _iProductPropertiesValueRepository;
        private readonly IFileService _iFileService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IProductService _iProductService;


        public ProductController(ILogger<ProductController> iLogger, IProductRepository iProductRepository,
            IProductPurposeRepository iProductPurposeRepository,
            IProductPropertiesRepository iProductPropertiesRepository,
            IProductCategoryRepository iProductCategoryRepository,
            IProductCategoryProductRepository iProductCategoryProductRepository,
            IFileService iFileService, IProductImageRepository iProductImageRepository,
            ApplicationDbContext applicationDbContext,
            IProductSimilarRepository iProductSimilarRepository,
            IProductSimilarPropertyRepository iProductSimilarPropertyRepository,
            IProductPropertiesValueRepository iProductPropertiesValueRepository, IProductService iProductService,
            ILoggingService iLoggingService)
        {
            _iLogger = iLogger;
            _iProductRepository = iProductRepository;
            _iProductPurposeRepository = iProductPurposeRepository;
            _iProductPropertiesRepository = iProductPropertiesRepository;
            _iProductCategoryRepository = iProductCategoryRepository;
            _iProductCategoryProductRepository = iProductCategoryProductRepository;
            _iFileService = iFileService;
            _iProductImageRepository = iProductImageRepository;
            _applicationDbContext = applicationDbContext;
            _iProductSimilarRepository = iProductSimilarRepository;
            _iProductSimilarPropertyRepository = iProductSimilarPropertyRepository;
            _iProductPropertiesValueRepository = iProductPropertiesValueRepository;
            _iProductService = iProductService;
            _iLoggingService = iLoggingService;
        }

        // GET
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Index(string txtSearch, int? status, int? statusTT, int pageindex = 1)
        {
            var query = _iProductRepository.GetProductAllIndex();
            if (!string.IsNullOrEmpty(txtSearch))
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.Name, "%" + txtSearch.Trim() + "%") ||
                    EF.Functions.Like(x.Sku, "%" + txtSearch.Trim() + "%"));
            }

            if (statusTT != null && statusTT != 0)
            {
                switch (statusTT)
                {
                    case 1:
                        query = query.Where(x => x.IsHot.Value);
                        break;
                    case 2:
                        query = query.Where(x => x.IsNew.Value);
                        break;
                    case 3:
                        query = query.Where(x => x.IsBestSale.Value);
                        break;
                    case 4:
                        query = query.Where(x => x.IsPromotion.Value);
                        break;
                }
            }

            if (status != null && status != 0)
            {
                switch (status)
                {
                    case 1:
                        query = query.Where(x => x.IsPublic.Value);
                        break;
                    case 2:
                        query = query.Where(x => !x.IsPublic.Value);
                        break;
                }
            }

            var listData = PagingList.Create(query.OrderByDescending(x => x.LastModifiedAt), PageSize, pageindex);
            listData.RouteValue = new RouteValueDictionary()
            {
                {"txtSearch", txtSearch},
                {"status", status},
                {"statusTT", statusTT},
            };
            ModelCollection model = new ModelCollection();
            model.AddModel("ListData", listData);
            model.AddModel("ListStatus", ProductConst.ListStatus);
            model.AddModel("ListStatusTT", ProductConst.ListStatusTT);
            model.AddModel("Page", pageindex);
            model.AddModel("isWareHouse",
                User.HasClaim(CmsClaimType.AreaControllerAction,
                    "Products@ProductController@WhHouseByProductIdSync".ToUpper()));
            return View(model);
        }

        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductController@Index")]
        [NonLoad]
        public JsonResult GetListProduct()
        {
            try
            {
                var query = _iProductRepository.FindAll();
                var model = PagingList.Create(query.OrderByDescending(x => x.Name), 20, 1);
                return Json(new
                {
                    code = 200,
                    msg = "successful",
                    content = model
                });
            }
            catch (Exception e)
            {
                _iLogger.LogInformation($" Xem chi tiết tx xuat khau loi {e.Message}");
                return Json(new
                {
                    code = 404,
                    msg = "fail",
                    content = new List<string>() {$"Lỗi hệ thống: {e.Message}"}
                });
            }
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Edit(int id)
        {
            ModelCollection model = new ModelCollection();
            model.AddModel("id", id);
            return View(model);
        }

        [HttpGet]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductController@Create")]
        [NonLoad]
        public JsonResult GetProductCategory()
        {
            try
            {
                var productCategory = _iProductCategoryRepository.FindAll().Where(x => x.Pid.HasValue && x.Pid != 0)
                    .OrderByDescending(x => x.Name).Select(x => new
                    {
                        Value = x.Id,
                        Label = x.Name
                    });
                return Json(new
                {
                    code = 200,
                    msg = "successful",
                    content = productCategory
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    msg = "fail",
                    content = "Lấy all productCategory lỗi!" + e.Message
                });
            }
        }


        [HttpGet]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductController@Create")]
        [NonLoad]
        public JsonResult SaveProductCategory(string name)
        {
            try
            {
                var productCategory = _iProductCategoryRepository.FindAll()
                    .Where(x => x.Name.ToUpper() == name.ToUpper()).ToList();
                if (productCategory.Count > 0)
                {
                    return Json(new
                    {
                        msg = "same",
                        content = "Trùng tên danh mục sản phẩm"
                    });
                }

                var category = new ProductCategory()
                {
                    Name = name.Trim(),
                    NonName = CmsFunction.RemoveUnicode(name.Trim()),
                    LastModifiedBy = UserInfo.UserId,
                    LastModifiedAt = DateTime.Now
                };
                _iProductCategoryRepository.Create(category);
                ILoggingService.Infor(this._iLogger, "Tạo mới danh mục sản phẩm thành công id:" + category.Id,
                    "UserId: " + UserInfo.UserId);

                var rs = _iProductCategoryRepository.FindAll()
                    .OrderByDescending(x => x.Name).Select(x => new
                    {
                        Value = x.Id,
                        Label = x.Name
                    });
                return Json(new
                {
                    code = 200,
                    msg = "successful",
                    content = rs
                });
            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "Tạo mới danh mục sản phẩm thành công",
                    "UserId: " + UserInfo.UserId, e);
                return Json(new
                {
                    msg = "fail",
                    content = "Lưu danh mục sản phẩm lỗi" + e.Message
                });
            }
        }

        [HttpGet]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductController@Create")]
        [NonLoad]
        public JsonResult SaveProductPurpose(string name)
        {
            try
            {
                var productPurpose = _iProductPurposeRepository.FindAll()
                    .Where(x => x.Name.ToUpper() == name.ToUpper()).ToList();
                if (productPurpose.Count > 0)
                {
                    return Json(new
                    {
                        msg = "same",
                        content = "Trùng tên mục đích sử dụng"
                    });
                }

                var purpose = new ProductPurpose()
                {
                    Name = name.Trim(),
                    LastModifiedBy = UserInfo.UserId,
                    LastModifiedAt = DateTime.Now
                };
                _iProductPurposeRepository.Create(purpose);
                ILoggingService.Infor(this._iLogger, "Tạo mới mục đích sử dụng thành công id:" + purpose.Id,
                    "UserId: " + UserInfo.UserId);

                var rs = _iProductPurposeRepository.FindAll()
                    .OrderByDescending(x => x.Name).Select(x => new
                    {
                        Value = x.Id,
                        Label = x.Name
                    });

                return Json(new
                {
                    code = 200,
                    msg = "successful",
                    content = rs
                });
            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "Tạo mới mục đích sử dụng lỗi ", "UserId: " + UserInfo.UserId, e);
                return Json(new
                {
                    msg = "fail",
                    content = "Lưu mục đích sử dụng lỗi" + e.Message
                });
            }
        }

        [HttpGet]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductController@Create")]
        [NonLoad]
        public JsonResult GetProductPurpose()
        {
            try
            {
                var productPurpose = _iProductPurposeRepository.FindAll()
                    .OrderByDescending(x => x.Name).Select(x => new
                    {
                        Value = x.Id,
                        Label = x.Name
                    }).ToList();
                return Json(new
                {
                    code = 200,
                    msg = "successful",
                    content = productPurpose
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    msg = "fail",
                    content = "Lấy all productCategory lỗi!" + e.Message
                });
            }
        }

        [NonLoad]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductController@Create")]
        public IActionResult SaveProduct([FromForm] CreateProductModel createData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var list = _iProductRepository.FindAll().Where(x => x.Sku.ToUpper() == createData.Sku.ToUpper())
                        .ToList();
                    if (list.Count > 0)
                    {
                        return Json(new
                        {
                            code = 404,
                            msg = "same",
                            content = "Trùng mã hàng"
                        });
                    }
                    //
                    ResultJson rs = _iProductService.SaveProduct(createData, UserInfo.UserId);
                    if (rs.StatusCode == 200)
                    {
                        ToastMessage(1, $"Thêm sản phẩm thành công !");
                        ILoggingService.Infor(this._iLogger, "Thêm sản phẩm thành công", "userId:" + UserInfo.UserId);

                        return Json(new
                        {
                            code = 200,
                            msg = "successful",
                            id = rs.Data
                        });
                    }
                    else
                    {
                        ToastMessage(-1, $"Thêm sản phẩm thành lỗi !");
                        ILoggingService.Error(this._iLogger, "Thêm sản phẩm lỗi", "id:" + UserInfo.UserId);

                        return Json(new
                        {
                            code = 404,
                            msg = "fail",
                            content = rs.Err
                        });
                    }
                }
                else
                {
                    ToastMessage(-1, $"Thêm mới sản phẩm lỗi !");
                    ILoggingService.Error(this._iLogger, "Thêm mới sản phẩm lỗi", "userId:" + UserInfo.UserId);
                    return Json(new
                    {
                        code = 404,
                        msg = "fail",
                        content = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage + "<br/>")
                            .Distinct().ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                ToastMessage(-1, $"Thêm mới sản phẩm lỗi !");
                ILoggingService.Error(this._iLogger, "Thêm mới sản phẩm lỗi", "userId:" + UserInfo.UserId, ex);
                _iLogger.LogInformation($" Lưu sản phẩm lỗi  {ex.Message}");
                return Json(new
                {
                    code = 404,
                    msg = "fail",
                    content = new List<string>() {$"Lỗi hệ thống: {ex.Message}"}
                });
            }
        }


        [NonLoad]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductController@Edit")]
        public IActionResult SaveProductEdit([FromForm] EditProductModel editData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var list = _iProductRepository.FindAll()
                        .Where(x => x.Sku.ToUpper() == editData.Sku.ToUpper() && x.Id != editData.Id)
                        .ToList();
                    if (list.Count > 0)
                    {
                        return Json(new
                        {
                            code = 404,
                            msg = "same",
                            content = "Trùng mã hàng"
                        });
                    }

                    ResultJson rs = _iProductService.SaveProductEdit(editData, UserInfo.UserId);
                    if (rs.StatusCode == 200)
                    {
                        ToastMessage(1, $"Sửa sản phẩm thành công !");
                        ILoggingService.Infor(this._iLogger, "Chỉnh sửa sản phẩm thành công", "id:" + editData.Id);

                        return Json(new
                        {
                            code = 200,
                            msg = "successful",
                            id = editData.Id
                        });
                    }
                    else
                    {
                        ToastMessage(-1, $"Sửa sản phẩm thành lỗi !");
                        ILoggingService.Error(this._iLogger, "Chỉnh sửa sản phẩm lỗi", "id:" + editData.Id);

                        return Json(new
                        {
                            code = 404,
                            msg = "fail",
                            content = rs.Err
                        });
                    }
                  
                }
                else
                {
                    ToastMessage(-1, $"Sửa sản phẩm thành lỗi !");
                    ILoggingService.Error(this._iLogger, "Chỉnh sửa sản phẩm lỗi", "id:" + editData.Id);

                    return Json(new
                    {
                        code = 404,
                        msg = "fail",
                        content = ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage + "<br/>")
                            .Distinct().ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                ToastMessage(-1, $"Sửa sản phẩm thành lỗi !");
                ILoggingService.Error(this._iLogger, "Chỉnh sửa sản phẩm lỗi", "id:" + editData.Id, ex);
                return Json(new
                {
                    code = 404,
                    msg = "fail",
                    content = new List<string>() {$"Lỗi hệ thống: {ex.Message}"}
                });
            }
        }


        [HttpGet]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Details(int id)
        {
            ModelCollection model = new ModelCollection();
            var product = _iProductRepository.FindById(id);
            model.AddModel("Product", product);
            var purposeName = _iProductPurposeRepository.FindById(product.ProductPurposeId.Value)?.Name ?? "";
            model.AddModel("Purpose", purposeName);
            var listCategory = _iProductCategoryRepository.GetListCategory(id).Select(x => x.Name).ToList();
            model.AddModel("Category", listCategory);
            var listImage = _iProductImageRepository.FindAll().Where(x => x.ProductId == id).ToList();
            model.AddModel("Images", listImage);
            var listPro = _iProductRepository.GetProductProperties(id);
            model.AddModel("ListPro", listPro);
            var listS = _iProductSimilarRepository.FindAll().Where(x => x.ProductId == id).ToList();
            model.AddModel("ListS", listS);
            model.AddModel("isWareHouse",
                User.HasClaim(CmsClaimType.AreaControllerAction,
                    "Products@ProductController@WhHouseByProductIdSync".ToUpper()));
            return View(model);
        }

        [HttpGet]
        [NonLoad]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductController@Edit")]
        public JsonResult GetProductEdit(int id)
        {
            try
            {
                var product = _iProductRepository.FindById(id);
                var listPro = _iProductRepository.GetProductProperties(id);
                var listS = _iProductSimilarRepository.FindAll().Where(x => x.ProductId == id).ToList();
                var listImage = _iProductImageRepository.FindAll().Where(x => x.ProductId == id).Select(x => x.Link)
                    .ToList();
                var listCategory = _iProductCategoryRepository.GetListCategory(id).Select(x => new
                {
                    Value = x.Id,
                    Label = x.Name
                }).ToList();
                ;
                return Json(new
                {
                    code = 200,
                    msg = "successful",
                    content1 = product,
                    content2 = listPro,
                    content3 = listS,
                    content4 = listCategory,
                    content5 = listImage
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    msg = "fail",
                    content = "Lấy all productCategory lỗi!" + e.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Policy = "PermissionMVC")]
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu sản phẩm");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                var product = _iProductRepository.FindById(id.Value);
                if (product != null)
                {
                    var productCategory = _iProductCategoryProductRepository.FindAll().Where(x => x.ProductId == id)
                        .ToList();
                    var productImage = _iProductImageRepository.FindAll().Where(x => x.ProductId == id).ToList();
                    var productProperties =
                        _iProductPropertiesRepository.FindAll().Where(x => x.ProductId == id).ToList();
                    var productProValue = _iProductPropertiesValueRepository.FindAll().Where(x => x.ProductId == id)
                        .ToList();
                    var productSi = _iProductSimilarRepository.FindAll().Where(x => x.ProductId == id).Select(x => x.Id)
                        .ToList();
                    var productSiValue = _iProductSimilarPropertyRepository.FindAll()
                        .Where(x => productSi.Contains(x.ProductSimilarId.Value)).ToList();

                    _iProductCategoryProductRepository.DeleteAll(productCategory);
                    _iProductImageRepository.DeleteAll(productImage);
                    _iProductSimilarPropertyRepository.DeleteAll(productSiValue);
                    _iProductPropertiesValueRepository.DeleteAll(productProValue);
                    _iProductPropertiesRepository.DeleteAll(productProperties);
                    _iProductSimilarRepository.DeleteAll(productSi);

                    _iProductRepository.Delete(product);
                    ILoggingService.Infor(this._iLogger, "Xóa sản phẩm thành công id:" + id,
                        "UserId: " + UserInfo.UserId);
                    ToastMessage(1, "Xóa sản phẩm thành công");
                    return Json(new
                    {
                        msg = "successful",
                        content = "Xóa sản phẩm thành công"
                    });
                }
                else
                {
                    ILoggingService.Error(this._iLogger, "Xóa sản phẩm lỗi" + "id:" + id, "UserId: " + UserInfo.UserId);
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
                ILoggingService.Error(this._iLogger, "Xóa sản phẩm lỗi" + "id:" + id, "UserId: " + UserInfo.UserId, ex);
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
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductController@Delete")]
        [NonLoad]
        public JsonResult DeleteAll(List<int> id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu sản phẩm");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                var productCategory = _iProductCategoryProductRepository.FindAll()
                    .Where(x => id.Contains(x.ProductId.Value))
                    .ToList();
                var productImage = _iProductImageRepository.FindAll().Where(x => id.Contains(x.ProductId)).ToList();
                var productProperties =
                    _iProductPropertiesRepository.FindAll().Where(x => id.Contains(x.ProductId.Value)).ToList();
                var productProValue = _iProductPropertiesValueRepository.FindAll()
                    .Where(x => id.Contains(x.ProductId.Value))
                    .ToList();
                var productSi = _iProductSimilarRepository.FindAll().Where(x => id.Contains(x.ProductId.Value))
                    .Select(x => x.Id).ToList();
                var productSiValue = _iProductSimilarPropertyRepository.FindAll()
                    .Where(x => productSi.Contains(x.ProductSimilarId.Value)).ToList();
                _iProductCategoryProductRepository.DeleteAll(productCategory);
                _iProductImageRepository.DeleteAll(productImage);
                _iProductSimilarPropertyRepository.DeleteAll(productSiValue);
                _iProductPropertiesValueRepository.DeleteAll(productProValue);
                _iProductPropertiesRepository.DeleteAll(productProperties);
                _iProductSimilarRepository.DeleteAll(productSi);
                int rs = this._iProductRepository.DeleteAll(id);
                ToastMessage(1, $"Xóa thành công {rs} bản ghi");
                this._iLogger.LogInformation($"Xóa thành công {rs} sản phẩm, UserId: " + UserInfo.UserId);
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

        [HttpPost]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductController@Delete")]
        [NonLoad]
        public JsonResult DeleteProductPurpose([FromBody] JObject obj)
        {
            try
            {
                var ids = obj["ids"];
                if (ids == null)
                {
                    return Json(new
                    {
                        msg = "fail",
                        content = "Không có dữ liệu, không thể xóa"
                    });
                }

                List<int> id = ids.Select(x => (int) x).ToList();
                string textRs = "";

                ;
                List<int> idNoD = _iProductRepository.FindAll().Where(x => id.Contains(x.ProductPurposeId!.Value))
                    .Select(x => x.ProductPurposeId.Value).ToList();
                List<string> listNameNoDe =
                    _iProductPurposeRepository.FindAll().Where(x => idNoD.Contains(x.Id)).Select(x => x.Name).ToList();
                List<int> idDelete = id.Where(x => !idNoD.Contains(x)).ToList();
                int rs = _iProductPurposeRepository.DeleteAll(idDelete);
                this._iLogger.LogInformation($"Xóa thành công {rs} mục đích sử dụng, UserId: " + UserInfo.UserId);
                var productPurpose = _iProductPurposeRepository.FindAll()
                    .OrderByDescending(x => x.Name).Select(x => new
                    {
                        Value = x.Id,
                        Label = x.Name
                    }).ToList();
                return Json(new
                {
                    msg = "successful",
                    content = productPurpose,
                    dataNoDe = listNameNoDe,
                    code = 200
                });
            }
            catch (Exception ex)
            {
                ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                this._iLogger.LogError($"Xóa dữ liệu lỗi, liên hệ người quản trị: id {0} , UserId: {UserInfo.UserId}");
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                });
            }
        }

        [HttpGet]
        [NonLoad]
        public IActionResult GetAllProduct()
        {
            try
            {
                var rs = _iProductRepository.FindAll().Where(x => x.IsPublic == true).OrderBy(x => x.Name).ToList();

                return Json(new
                {
                    msg = "successful",
                    content = rs,
                    code = 200
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi "
                });
            }
        }
     
        [HttpGet]
        [NonLoad]
        public IActionResult ApiDetail(int? id)
        {
            try
            {
                var p = this._iProductRepository.FindByDetailId(id!.Value);
                if (p != null)
                {
                    double? priceStart = 0;
                    string? showPrice = null;
                    if (p.ProductSimilar.Count is > 0 and 1)
                    {
                        priceStart = p.ProductSimilar.FirstOrDefault()?.Price;
                        showPrice = CmsFunction.NumberFormatShow(priceStart);
                    }

                    if (p.ProductSimilar.Count is > 1)
                    {
                        priceStart = p.ProductSimilar.MinBy(x => x.Price)?.Price;
                        var priceEnd = p.ProductSimilar.MaxBy(x => x.Price)?.Price;
                        if (priceStart == priceEnd)
                        {
                            showPrice = CmsFunction.NumberFormatShow(priceStart);
                        }
                        else
                        {
                            showPrice =
                                $"{CmsFunction.NumberFormatShow(priceStart)} - {CmsFunction.NumberFormatShow(priceEnd)}";
                        }
                    }

                    return Json(
                        new
                        {
                            msg = "successful",
                            content = new
                            {
                                p.ProductProperties,
                                p.ProductSimilar,
                                p.Lead,
                                p.Image,
                                showPrice = showPrice,
                                p.Id,
                                p.Sku,
                                p.Name,
                                PriceSale = p.PriceSale,
                                p.Weight,
                                quantityUse = p.ProductSimilar.Sum(x => x.QuantityUse),
                                quantityKW = p.ProductSimilar.Sum(x => x.QuantityWh),
                                productRateCount = Math.Round(p.Rate ?? 0),
                                productCommentCount = 0,
                            },
                            code = 200
                        });
                }

                return Json(new
                {
                    code = 500,
                    msg = "fail",
                    content = ""
                });
            }
            catch (Exception ex)
            {
                this._iLogger.LogError($"Lấy dữ liệu chi tiết sản phẩm lỗi , UserId: {UserInfo.UserId} . {ex.Message}");
                return Json(new
                {
                    code = 500,
                    msg = "fail",
                    content = ""
                });
            }
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult WhHouseByProductIdSync(int id)
        {
            try
            {
                var listData = this._iProductSimilarRepository.FindAllByProductId(id);
                List<ProductSimilar> ListProductSimilarUpdate = new List<ProductSimilar>();
                if (listData.Count > 0)
                {
                    foreach (var item in listData)
                    {
                        if (!string.IsNullOrEmpty(item.Skuwh))
                        {
                            var rs = this._iProductService.GetWareHouseByCode(item.Skuwh);
                            if (rs != null)
                            {
                                if (rs.Cost > 0)
                                {
                                    item.Price = rs.Cost;
                                }

                                item.QuantityWh = rs.Quantity;
                                ListProductSimilarUpdate.Add(item);
                            }
                        }
                    }
                }

                if (ListProductSimilarUpdate.Count > 0)
                {
                    this._iProductSimilarRepository.BulkUpdate(ListProductSimilarUpdate);
                }

                ToastMessage(1, $"Cập nhật số lượng và giá sản phẩm trong kho vào hệ thống {AppConst.AppName} thành công");
                return Json(new
                {
                    msg = "successful",
                    content = "Cập nhật số lượng từ kho cho sản phẩm thành công",
                    code = 200
                });
            }
            catch (Exception ex)
            {
                this._iLogger.LogError(ex, $"Cập nhật số lượng từ kho cho sản phẩm {id} lỗi");
                // ignored
                return Json(new
                {
                    msg = "fail",
                    content = "Cập nhật số lượng từ kho cho sản phẩm lỗi",
                    code = 404
                });
            }
        }

        [NonLoad]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Products@ProductController@WhHouseByProductIdSync")]
        public IActionResult WhHouseByProductIdSyncAll(List<int> id)
        {
            try
            {
                if (id.Count > 0)
                {
                    var rs = this._iProductSimilarRepository.FindAllByListProductId(id);
                    List<ProductSimilar> listUpdate = new List<ProductSimilar>();
                    if (rs.Count > 0)
                    {
                        foreach (var item in rs)
                        {
                            var data = this._iProductService.GetWareHouseByCode(item.Skuwh);
                            if (data != null)
                            {
                                if (data.Cost > 0)
                                {
                                    item.Price = data.Cost;
                                }

                                item.QuantityWh = data.Quantity;
                                listUpdate.Add(item);
                            }
                        }
                    }

                    if (listUpdate.Count > 0)
                    {
                        this._iProductSimilarRepository.BulkUpdate(listUpdate);
                    }

                    this._iLoggingService.Infor(_iLogger, "Cập nhật số lượng từ kho cho sản phẩm thành công", $"{id}");
                    return Json(new
                    {
                        msg = "successful",
                        content = "Cập nhật số lượng từ kho cho sản phẩm lỗi",
                        code = 200
                    });
                }
                else
                {
                    return Json(new
                    {
                        msg = "fail",
                        content = "Vui lòng chọn sản phẩm",
                        code = 400
                    });
                }
            }
            catch (Exception ex)
            {
                this._iLoggingService.Error(_iLogger, "Cập nhật số lượng từ kho cho sản phẩm thành công", $"{id}", ex);
                return Json(new
                {
                    msg = "fail",
                    content = "Cập nhật số lượng từ kho cho sản phẩm lỗi",
                    code = 404
                });
            }
        }
    }
}