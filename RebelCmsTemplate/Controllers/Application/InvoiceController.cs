using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Repository.Application;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Api.Application;

[Route("api/application/[controller]")]
[ApiController]
public class InvoiceController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public InvoiceController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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

        InvoiceRepository invoiceRepository = new(_httpContextAccessor);
        var content = invoiceRepository.GetExcel();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "invoice6.xlsx");
    }

    [HttpPost]
    public ActionResult Post()
    {
        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);
        InvoiceRepository invoiceRepository = new(_httpContextAccessor);
        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);
        var invoiceKey = !string.IsNullOrEmpty(Request.Form["invoiceKey"])
            ? Convert.ToInt32(Request.Form["invoiceKey"])
            : 0;
        var customerKey = !string.IsNullOrEmpty(Request.Form["customerKey"])
            ? Convert.ToInt32(Request.Form["customerKey"])
            : 0;
        var shipperKey = !string.IsNullOrEmpty(Request.Form["shipperKey"])
            ? Convert.ToInt32(Request.Form["shipperKey"])
            : 0;
        var employeeKey = !string.IsNullOrEmpty(Request.Form["employeeKey"])
            ? Convert.ToInt32(Request.Form["employeeKey"])
            : 0;
        var invoiceOrderDate = DateOnly.FromDateTime(DateTime.Now);
        if (!string.IsNullOrEmpty(Request.Form["invoiceOrderDate"]))
        {
            var dateString = Request.Form["invoiceOrderDate"].ToString().Split("-");
            invoiceOrderDate = new DateOnly(Convert.ToInt32(dateString[0]), Convert.ToInt32(dateString[1]),
                Convert.ToInt32(dateString[2]));
        }

        var invoiceRequiredDate = DateOnly.FromDateTime(DateTime.Now);
        if (!string.IsNullOrEmpty(Request.Form["invoiceRequiredDate"]))
        {
            var dateString = Request.Form["invoiceRequiredDate"].ToString().Split("-");
            invoiceRequiredDate = new DateOnly(Convert.ToInt32(dateString[0]), Convert.ToInt32(dateString[1]),
                Convert.ToInt32(dateString[2]));
        }

        var invoiceShippedDate = DateOnly.FromDateTime(DateTime.Now);
        if (!string.IsNullOrEmpty(Request.Form["invoiceShippedDate"]))
        {
            var dateString = Request.Form["invoiceShippedDate"].ToString().Split("-");
            invoiceShippedDate = new DateOnly(Convert.ToInt32(dateString[0]), Convert.ToInt32(dateString[1]),
                Convert.ToInt32(dateString[2]));
        }

        var invoiceFreight = !string.IsNullOrEmpty(Request.Form["invoiceFreight"])
            ? Convert.ToDecimal(Request.Form["invoiceFreight"])
            : 0;
        var invoiceShipName = Request.Form["invoiceShipName"];
        var invoiceShipAddress = Request.Form["invoiceShipAddress"];
        var invoiceShipCity = Request.Form["invoiceShipCity"];
        var invoiceShipRegion = Request.Form["invoiceShipRegion"];
        var invoiceShipPostalCode = Request.Form["invoiceShipPostalCode"];
        var invoiceShipCountry = Request.Form["invoiceShipCountry"];
        var search = Request.Form["search"];
        List<InvoiceModel> data = new();
        InvoiceModel dataSingle = new();
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
                        InvoiceModel invoiceModel = new()
                        {
                            CustomerKey = customerKey,
                            ShipperKey = shipperKey,
                            EmployeeKey = employeeKey,
                            InvoiceOrderDate = invoiceOrderDate,
                            InvoiceRequiredDate = invoiceRequiredDate,
                            InvoiceShippedDate = invoiceShippedDate,
                            InvoiceFreight = invoiceFreight,
                            InvoiceShipName = invoiceShipName,
                            InvoiceShipAddress = invoiceShipAddress,
                            InvoiceShipCity = invoiceShipCity,
                            InvoiceShipRegion = invoiceShipRegion,
                            InvoiceShipPostalCode = invoiceShipPostalCode,
                            InvoiceShipCountry = invoiceShipCountry
                        };
                        lastInsertKey = invoiceRepository.Create(invoiceModel);
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
                        data = invoiceRepository.Read();
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
                        data = invoiceRepository.Search(search);
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
            case "single":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        InvoiceModel invoiceModel = new()
                        {
                            InvoiceKey = invoiceKey
                        };
                        dataSingle = invoiceRepository.GetSingle(invoiceModel);
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
            case "singleWithDetail":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        InvoiceModel invoiceModel = new()
                        {
                            InvoiceKey = invoiceKey
                        };
                        dataSingle = invoiceRepository.GetSingleWithDetail(invoiceModel);
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
                        InvoiceModel invoiceModel = new()
                        {
                            InvoiceKey = invoiceKey,
                            CustomerKey = customerKey,
                            ShipperKey = shipperKey,
                            EmployeeKey = employeeKey,
                            InvoiceOrderDate = invoiceOrderDate,
                            InvoiceRequiredDate = invoiceRequiredDate,
                            InvoiceShippedDate = invoiceShippedDate,
                            InvoiceFreight = invoiceFreight,
                            InvoiceShipName = invoiceShipName,
                            InvoiceShipAddress = invoiceShipAddress,
                            InvoiceShipCity = invoiceShipCity,
                            InvoiceShipRegion = invoiceShipRegion,
                            InvoiceShipPostalCode = invoiceShipPostalCode,
                            InvoiceShipCountry = invoiceShipCountry
                        };
                        invoiceRepository.Update(invoiceModel);
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
                        InvoiceModel invoiceModel = new()
                        {
                            InvoiceKey = invoiceKey
                        };
                        invoiceRepository.Delete(invoiceModel);
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