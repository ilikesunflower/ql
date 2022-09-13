using System;
using CMS.Controllers;
using CMS_Access.Repositories.Categories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using CMS_Lib.Extensions.Attribute;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Categories.Controllers;

[Area("Categories")]
[NonLoad]
public class DistrictController : BaseController
{
    private readonly IDistrictRepository _iDistrictRepository;
    private readonly ICommuneRepository _iCommuneRepository;
    private readonly ILogger _iLogger;

    public DistrictController(IDistrictRepository iDistrictRepository, ICommuneRepository iCommuneRepository,  ILogger<DistrictController> iLogger)
    {
        _iDistrictRepository = iDistrictRepository;
        _iCommuneRepository = iCommuneRepository;
        _iLogger = iLogger;

    }

    [HttpGet]
    public  IActionResult ListAddressDistrict(string code)
        {
            try
            {
                var districts = _iDistrictRepository.FindAll().Where(x => x.ProvinceCode == code).ToList();
                return Json(new
                {
                    code = 200,
                    content = districts,
                    msg ="Lấy d/s districts thành công"
                });
            }
            catch (Exception e)
            {
                _iLogger.LogError(e.Message);
                return Json(new
                {   code = 400,
                    msg = "not found",
                    content = ""
                });
            }
        
        }
}