using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Repository.Application;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Api.Application;

[Route("api/application/[controller]")]
[ApiController]
public class SupplierController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public SupplierController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
    {
        _renderViewToStringUtil = renderViewToStringUtil;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        SharedUtil sharedUtils = new(_httpContextAccessor);
        if (sharedUtils.GetTenantId() == 0 || sharedUtils.GetTenantId().Equals(null))
        {
            const string? templatePath = "~/Views/Error/403.cshtml";
            var page = await _renderViewToStringUtil.RenderViewToStringAsync(ControllerContext, templatePath);
            return Ok(page);
        }

        SupplierRepository supplierRepository = new(_httpContextAccessor);
        var content = supplierRepository.GetExcel();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "supplier15.xlsx");
    }

    [HttpPost]
    public ActionResult Post()
    {
        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);
        SupplierRepository supplierRepository = new(_httpContextAccessor);
        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);
        var supplierKey = !string.IsNullOrEmpty(Request.Form["supplierKey"])
            ? Convert.ToInt32(Request.Form["supplierKey"])
            : 0;
        var supplierName = Request.Form["supplierName"];
        var supplierContactName = Request.Form["supplierContactName"];
        var supplierContactTitle = Request.Form["supplierContactTitle"];
        var supplierAddress = Request.Form["supplierAddress"];
        var supplierCity = Request.Form["supplierCity"];
        var supplierRegion = Request.Form["supplierRegion"];
        var supplierPostalCode = Request.Form["supplierPostalCode"];
        var supplierCountry = Request.Form["supplierCountry"];
        var supplierPhone = Request.Form["supplierPhone"];
        var supplierFax = Request.Form["supplierFax"];
        var supplierHomePage = Request.Form["supplierHomePage"];
        var search = Request.Form["search"];
        List<SupplierModel> data = new();
        SupplierModel dataSingle = new();
        string code;
        var lastInsertKey = 0;
        switch (mode)
        {
            case "create":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.CREATE_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        SupplierModel supplierModel = new()
                        {
                            SupplierName = supplierName,
                            SupplierContactName = supplierContactName,
                            SupplierContactTitle = supplierContactTitle,
                            SupplierAddress = supplierAddress,
                            SupplierCity = supplierCity,
                            SupplierRegion = supplierRegion,
                            SupplierPostalCode = supplierPostalCode,
                            SupplierCountry = supplierCountry,
                            SupplierPhone = supplierPhone,
                            SupplierFax = supplierFax,
                            SupplierHomePage = supplierHomePage,
                        };
                        lastInsertKey = supplierRepository.Create(supplierModel);
                        code = ((int) ReturnCodeEnum.CREATE_SUCCESS).ToString();
                        status = true;
                    }
                    catch (Exception ex)
                    {
                        code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                            ? ex.Message
                            : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                    }
                }

                break;
            case "read":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        data = supplierRepository.Read();
                        code = ((int) ReturnCodeEnum.CREATE_SUCCESS).ToString();
                        status = true;
                    }
                    catch (Exception ex)
                    {
                        code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                            ? ex.Message
                            : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                    }
                }

                break;
            case "search":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        code = ((int) ReturnCodeEnum.READ_SUCCESS).ToString();
                        status = true;
                    }
                    catch (Exception ex)
                    {
                        data = supplierRepository.Search(search);
                        code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                            ? ex.Message
                            : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                    }
                }

                break;
            case "single":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        SupplierModel supplierModel = new()
                        {
                            SupplierKey = supplierKey
                        };
                        dataSingle = supplierRepository.GetSingle(supplierModel);
                        code = ((int) ReturnCodeEnum.READ_SUCCESS).ToString();
                        status = true;
                    }
                    catch (Exception ex)
                    {
                        code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                            ? ex.Message
                            : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                    }
                }

                break;
            case "update":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.UPDATE_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        SupplierModel supplierModel = new()
                        {
                            SupplierKey = supplierKey,
                            SupplierName = supplierName,
                            SupplierContactName = supplierContactName,
                            SupplierContactTitle = supplierContactTitle,
                            SupplierAddress = supplierAddress,
                            SupplierCity = supplierCity,
                            SupplierRegion = supplierRegion,
                            SupplierPostalCode = supplierPostalCode,
                            SupplierCountry = supplierCountry,
                            SupplierPhone = supplierPhone,
                            SupplierFax = supplierFax,
                            SupplierHomePage = supplierHomePage,
                        };
                        supplierRepository.Update(supplierModel);
                        code = ((int) ReturnCodeEnum.UPDATE_SUCCESS).ToString();
                        status = true;
                    }
                    catch (Exception ex)
                    {
                        code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                            ? ex.Message
                            : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                    }
                }

                break;
            case "delete":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.DELETE_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        SupplierModel supplierModel = new()
                        {
                            SupplierKey = supplierKey
                        };
                        supplierRepository.Delete(supplierModel);
                        code = ((int) ReturnCodeEnum.DELETE_SUCCESS).ToString();
                        status = true;
                    }
                    catch (Exception ex)
                    {
                        code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                            ? ex.Message
                            : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                    }
                }

                break;
            default:
                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                break;
        }

        if (data.Count > 0)
        {
            return Ok(new {status, code, data});
        }

        if (mode.Equals("single") || mode.Equals("singleWithDetail"))
        {
            return Ok(new {status, code, dataSingle});
        }

        return lastInsertKey > 0 ? Ok(new {status, code, lastInsertKey}) : Ok(new {status, code});
    }
}