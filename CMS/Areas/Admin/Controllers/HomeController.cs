using System;
using System.Linq;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Util;
using CMS.Areas.Admin.Const;
using CMS.Areas.Admin.Services.Home;
using CMS.Areas.Admin.ViewModels.Home;
using CMS.Areas.Admin.ViewModels.Home.SaleGroup;
using CMS.Areas.Admin.ViewModels.Home.Sales;
using CMS.Areas.Admin.ViewModels.Home.ToProduct;
using CMS.Controllers;
using CMS.DataTypes;
using CMS.Extensions.Validate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Obsolete]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _iLogger;
        private readonly IDashBoardService _iDashBoardService;


        public HomeController(ILogger<HomeController> iLogger, IDashBoardService iDashBoardService)
        {
            this._iLogger = iLogger;
            this._iDashBoardService = iDashBoardService;
        }
        
        [HttpGet]
        [NoActiveMenu]
        [NonLoad]
        public IActionResult Index()
        {
            IndexViewModel rs = new IndexViewModel();
            rs.IsDataSales = User.HasClaim(CmsClaimType.AreaControllerAction,
                "Admin@HomeController@GetChartDataSales".ToUpper());
            rs.IsProductBest = User.HasClaim(CmsClaimType.AreaControllerAction,
                "Admin@HomeController@GetChartToProduct".ToUpper());
            rs.IsGroupCustomer = User.HasClaim(CmsClaimType.AreaControllerAction,
                "Admin@HomeController@GetChartDataSaleGroup".ToUpper());
            rs.IsDataAreas = User.HasClaim(CmsClaimType.AreaControllerAction,
                "Admin@HomeController@GetChartArea".ToUpper());
            rs.IsProductRating = User.HasClaim(CmsClaimType.AreaControllerAction,
                "Admin@HomeController@GetToRating".ToUpper());
            rs.IsCustomerActive = User.HasClaim(CmsClaimType.AreaControllerAction,
                "Reports@CustomerActivityController@WidgetDashboard".ToUpper());
            return View(rs); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        [Consumes("application/json")]
        [AllowParameter]
        public IActionResult GetChartDataSales([Bind("TimeFlow","DateStart", "DateEnd")] [FromBody] SalesFilterViewModel model)
        {
            try
            {
                
                var time = new TimeRange(model.TimeFlow, model.DateStart, model.DateEnd);
                CharDataModel rs;
                if ("days" == model.TimeFlow)
                {
                    rs = _iDashBoardService.GetDataSalesDay(time.Start, time.End);
                }
                else
                {
                    rs = _iDashBoardService.GetDataSalesMonth(time.Start, time.End);
                }
                return Ok(new
                {
                    code = 200,
                    msg = "successful",
                    content = rs
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 500,
                    msg = "fail",
                    content = e.Message
                });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        [Consumes("application/json")]
        [AllowParameter]
        public IActionResult GetChartDataSaleGroup([Bind("DateStart","DateEnd")] [FromBody] SaleGroupViewModel model)
        {
            try
            {
                var time = new TimeRange( model.DateStart, model.DateEnd);
                CharDataModel rs = new CharDataModel();
                rs = _iDashBoardService.GetDataSaleGroup(time.Start, time.End);
                return Ok(new
                {
                    code = 200,
                    msg = "successful",
                    content = rs
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 500,
                    msg = "fail",
                    content = e.Message
                });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        [Consumes("application/json")]
        [AllowParameter]
        public IActionResult GetChartToProduct([Bind("FilterStatus","DateStart","DateEnd")] [FromBody] ToProductViewModel model)
        {
            try
            {
                var time = new TimeRange(model.DateStart, model.DateEnd);
                CharDataToProductModel rs = new CharDataToProductModel();
                rs = _iDashBoardService.GetDataToProduct(time.Start, time.End, model.FilterStatus,20);
                return Ok(new
                {
                    code = 200,
                    msg = "successful",
                    content = rs,
                    filter = FilterToProductConst.GetStringStatus,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 500,
                    msg = "fail",
                    content = e.Message
                });
            }
        } 
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        [Consumes("application/json")]
        [AllowParameter]
        public IActionResult GetChartArea([Bind("DateStart","DateEnd")] [FromBody] SaleGroupViewModel model)
        {
            try
            {
                var time = new TimeRange(model.DateStart, model.DateEnd);
                var listGroup1 = _iDashBoardService.GetDataArea(time.Start, time.End);
                return Ok(new
                {
                    code = 200,
                    msg = "successful",
                    content = new {
                        listGroup1 = listGroup1.Select(x => new SeriesChart()
                        {
                            Name = x.Name,
                            Y = x.Price
                        }).ToList(),
                        listGroup2 = listGroup1.Select(x => new SeriesChart()
                        {
                            Name = x.Name,
                            Y = x.Quantity
                        }).ToList()
                    },
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 500,
                    msg = "fail",
                    content = e.Message
                });
            }
        }  
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        [Consumes("application/json")]
        [AllowParameter]
        public IActionResult GetToRating([Bind("DateStart","DateEnd")] [FromBody] SaleGroupViewModel model)
        {
            try
            {
                var time = new TimeRange(model.DateStart, model.DateEnd);
                var rs = _iDashBoardService.GetDataToRating(time.Start, time.End);
                return Ok(new
                {
                    code = 200,
                    msg = "successful",
                    content = rs,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    code = 500,
                    msg = "fail",
                    content = e.Message
                });
            }
        }
        
    }
}