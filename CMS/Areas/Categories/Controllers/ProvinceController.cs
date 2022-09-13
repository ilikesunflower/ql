using System;
using System.Linq;
using CMS.Controllers;
using CMS_Access.Repositories.Categories;
using CMS_Lib.Extensions.Attribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Categories.Controllers;
[Area("Categories")]
[NonLoad]
public class ProvinceController: BaseController
{
    private readonly IProvinceRepository _iProvinceRepository;
    private readonly IDistrictRepository _iDistrictRepository;
    private readonly ILogger _iLogger;

    public ProvinceController(IProvinceRepository iProvinceRepository, IDistrictRepository iDistrictRepository, ILogger<ProvinceController> iLogger)
    {
        _iProvinceRepository = iProvinceRepository;
        _iDistrictRepository = iDistrictRepository;
        _iLogger = iLogger;
    }
    public  IActionResult ListAddressProvince()
    {
        try
        {
            var province = _iProvinceRepository.FindAll().ToList();
            return Json(new
            {
                code = 200,
                content = province,
                msg ="Lấy d/s province thành công"
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