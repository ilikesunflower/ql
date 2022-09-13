
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
public class CommuneController: BaseController
{
    private readonly ICommuneRepository _iCommuneRepository;
    private readonly ILogger _iLogger;

    public CommuneController(ICommuneRepository iCommuneRepository , ILogger<CommuneController> iLogger)
    {
        _iCommuneRepository = iCommuneRepository;
        _iLogger = iLogger;
    }
    [HttpGet]
    public  IActionResult ListAddressCommune(string code)
    {
        try
        {
            var commune = _iCommuneRepository.FindAll().Where(x => x.DistrictCode == code).ToList();
            return Json(new
            {
                code = 200,
                content = commune,
                msg ="Lấy d/s commune thành công"
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