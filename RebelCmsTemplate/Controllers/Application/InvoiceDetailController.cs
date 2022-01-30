using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Repository.Application;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Application;

[Route("api/application/[controller]")]
[ApiController]
public class InvoiceDetailController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public InvoiceDetailController(RenderViewToStringUtil renderViewToStringUtil,
        IHttpContextAccessor httpContextAccessor)
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

        InvoiceDetailRepository invoiceDetailRepository = new(_httpContextAccessor);
        var content = invoiceDetailRepository.GetExcel();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "invoiceDetail39.xlsx");
    }

    [HttpPost]
    public ActionResult Post()
    {
        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);
        InvoiceDetailRepository invoiceDetailRepository = new(_httpContextAccessor);
        ProductRepository productRepository = new(_httpContextAccessor);
        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);
        
        string? code;
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
                        if (!uint.TryParse(Request.Form["invoiceKey"], out var invoiceKey))
                        {
                            code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                            return Ok(new {status, code});
                        }

                        if (!uint.TryParse(Request.Form["productKey"], out var productKey))
                        {
                            code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                            return Ok(new {status, code});
                        }
                        if(productKey == 0 )
                        {
                            productKey = productRepository.GetDefault();
                        }

                        var invoiceDetailUnitPrice = !string.IsNullOrEmpty(Request.Form["invoiceDetailUnitPrice"])
                            ? Convert.ToDecimal(Request.Form["invoiceDetailUnitPrice"])
                            : 0;
                        var invoiceDetailQuantity = !string.IsNullOrEmpty(Request.Form["invoiceDetailQuantity"])
                            ? Convert.ToInt32(Request.Form["invoiceDetailQuantity"])
                            : 0;
                        var invoiceDetailDiscount = !string.IsNullOrEmpty(Request.Form["invoiceDetailDiscount"])
                            ? Convert.ToDouble(Request.Form["invoiceDetailDiscount"])
                            : 0;

                        InvoiceDetailModel invoiceDetailModel = new()
                        {
                            InvoiceKey = invoiceKey,
                            ProductKey = productKey,
                            InvoiceDetailUnitPrice = invoiceDetailUnitPrice,
                            InvoiceDetailQuantity = invoiceDetailQuantity,
                            InvoiceDetailDiscount = invoiceDetailDiscount
                        };
                        var lastInsertKey = invoiceDetailRepository.Create(invoiceDetailModel);
                        code = ((int) ReturnCodeEnum.CREATE_SUCCESS).ToString();
                        status = true;
                        return Ok(new {status, code, lastInsertKey});
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
                        var data = invoiceDetailRepository.Read();
                        code = ((int) ReturnCodeEnum.CREATE_SUCCESS).ToString();
                        status = true;
                        return Ok(new {status, code, data});
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
                    if (!string.IsNullOrEmpty(Request.Form["search"]))
                    {
                        try
                        {
                            var search = Request.Form["search"];

                            var data = invoiceDetailRepository.Search(search);
                            code = ((int) ReturnCodeEnum.READ_SUCCESS).ToString();
                            status = true;
                            return Ok(new {status, code, data});
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                                ? ex.Message
                                : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    else
                    {
                        code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
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
                    if (!string.IsNullOrEmpty(Request.Form["invoiceDetailKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["invoiceDetailKey"], out var invoiceDetailKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (invoiceDetailKey > 0)
                            {
                                if (!uint.TryParse(Request.Form["invoiceKey"], out var invoiceKey))
                                {
                                    code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                    return Ok(new {status, code});
                                }

                                if (!uint.TryParse(Request.Form["productKey"], out var productKey))
                                {
                                    code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                    return Ok(new {status, code});
                                }
                                if (productKey == 0 )
                                {
                                    productKey = productRepository.GetDefault();
                                }

                                var invoiceDetailUnitPrice =
                                    !string.IsNullOrEmpty(Request.Form["invoiceDetailUnitPrice"])
                                        ? Convert.ToDecimal(Request.Form["invoiceDetailUnitPrice"])
                                        : 0;
                                var invoiceDetailQuantity = !string.IsNullOrEmpty(Request.Form["invoiceDetailQuantity"])
                                    ? Convert.ToInt32(Request.Form["invoiceDetailQuantity"])
                                    : 0;
                                var invoiceDetailDiscount = !string.IsNullOrEmpty(Request.Form["invoiceDetailDiscount"])
                                    ? Convert.ToDouble(Request.Form["invoiceDetailDiscount"])
                                    : 0;
                                InvoiceDetailModel invoiceDetailModel = new()
                                {
                                    InvoiceDetailKey = invoiceDetailKey,
                                    InvoiceKey = invoiceKey,
                                    ProductKey = productKey,
                                    InvoiceDetailUnitPrice = invoiceDetailUnitPrice,
                                    InvoiceDetailQuantity = invoiceDetailQuantity,
                                    InvoiceDetailDiscount = invoiceDetailDiscount
                                };
                                invoiceDetailRepository.Update(invoiceDetailModel);
                                code = ((int) ReturnCodeEnum.UPDATE_SUCCESS).ToString();
                                status = true;
                            }
                            else
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                                ? ex.Message
                                : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    else
                    {
                        code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
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
                    if (!string.IsNullOrEmpty(Request.Form["invoiceDetailKey"]))
                    {
                        try
                        {
                            if (!uint.TryParse(Request.Form["invoiceDetailKey"], out var invoiceDetailKey))
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new {status, code});
                            }

                            if (invoiceDetailKey > 0)
                            {
                                InvoiceDetailModel invoiceDetailModel = new()
                                {
                                    InvoiceDetailKey = invoiceDetailKey
                                };
                                invoiceDetailRepository.Delete(invoiceDetailModel);
                                code = ((int) ReturnCodeEnum.DELETE_SUCCESS).ToString();
                                status = true;
                            }
                            else
                            {
                                code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int) AccessEnum.ADMINISTRATOR_ACCESS
                                ? ex.Message
                                : ((int) ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    else
                    {
                        code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                    }
                }

                break;
            default:
                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                break;
        }

        return Ok(new {status, code});
    }
}