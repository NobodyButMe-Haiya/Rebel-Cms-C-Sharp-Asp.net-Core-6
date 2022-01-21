
using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Repository.Application;
using RebelCmsTemplate.Util;
using System.Globalization;
namespace RebelCmsTemplate.Controllers.Api.Application;
[Route("api/application/[controller]")]
[ApiController]
public class InvoiceDetailController : Controller {
 private readonly IHttpContextAccessor _httpContextAccessor;
 private readonly RenderViewToStringUtil _renderViewToStringUtil;
 public InvoiceDetailController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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
   return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","invoiceDetail39.xlsx");
  }
  [HttpPost]
  public ActionResult Post()
  {
	var status = false;
	var mode = Request.Form["mode"];
	var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);
           InvoiceDetailRepository invoiceDetailRepository = new(_httpContextAccessor);
            SharedUtil sharedUtil = new(_httpContextAccessor);
            CheckAccessUtil checkAccessUtil = new (_httpContextAccessor);
	int invoiceDetailKey =  !string.IsNullOrEmpty(Request.Form["invoiceDetailKey"])?Convert.ToInt32(Request.Form["invoiceDetailKey"]):0;
	int invoiceKey =  !string.IsNullOrEmpty(Request.Form["invoiceKey"])?Convert.ToInt32(Request.Form["invoiceKey"]):0;
	int productKey =  !string.IsNullOrEmpty(Request.Form["productKey"])?Convert.ToInt32(Request.Form["productKey"]):0;
	decimal invoiceDetailUnitPrice =  !string.IsNullOrEmpty(Request.Form["invoiceDetailUnitPrice"])?Convert.ToDecimal(Request.Form["invoiceDetailUnitPrice"]):0;
	int invoiceDetailQuantity =  !string.IsNullOrEmpty(Request.Form["invoiceDetailQuantity"])?Convert.ToInt32(Request.Form["invoiceDetailQuantity"]):0;
	double invoiceDetailDiscount =  !string.IsNullOrEmpty(Request.Form["invoiceDetailDiscount"])?Convert.ToDouble(Request.Form["invoiceDetailDiscount"]):0;
            var search = Request.Form["search"];
           List<InvoiceDetailModel> data = new();
           InvoiceDetailModel dataSingle = new();
            string code;
            var lastInsertKey = 0;
            switch (mode)
            {
                case "create":
                    if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.CREATE_ACCESS))
                    {
                        code = ((int)ReturnCodeEnum.ACCESS_DENIED).ToString();
                    }
                    else
                    {
                        try
                        {
                            InvoiceDetailModel invoiceDetailModel = new()
                            {
			InvoiceKey = invoiceKey,
			ProductKey = productKey,
			InvoiceDetailUnitPrice = invoiceDetailUnitPrice,
			InvoiceDetailQuantity = invoiceDetailQuantity,
			InvoiceDetailDiscount = invoiceDetailDiscount,
                            };
                           lastInsertKey = invoiceDetailRepository.Create(invoiceDetailModel);
                            code = ((int)ReturnCodeEnum.CREATE_SUCCESS).ToString();
                            status = true;
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int)AccessEnum.ADMINISTRATOR_ACCESS ? ex.Message : ((int)ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    break;
                case "read":
                    if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                    {
                        code = ((int)ReturnCodeEnum.ACCESS_DENIED).ToString();
                    }
                    else
                    {
                        try
                        {
                           data = invoiceDetailRepository.Read();
                            code = ((int)ReturnCodeEnum.CREATE_SUCCESS).ToString();
                            status = true;
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int)AccessEnum.ADMINISTRATOR_ACCESS ? ex.Message : ((int)ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    break;
                case "search":
                    if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                    {
                        code = ((int)ReturnCodeEnum.ACCESS_DENIED).ToString();
                    }
                    else
                    {
                        try
                        {
                            code = ((int)ReturnCodeEnum.READ_SUCCESS).ToString();
                            status = true;
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int)AccessEnum.ADMINISTRATOR_ACCESS ? ex.Message : ((int)ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    break;
                case "single":
                    if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                    {
                        code = ((int)ReturnCodeEnum.ACCESS_DENIED).ToString();
                    }
                    else
                    {
                        try
                        {
                            InvoiceDetailModel invoiceDetailModel = new()
                            {
                                InvoiceDetailKey = invoiceDetailKey
                            };
                           dataSingle = invoiceDetailRepository.GetSingle(invoiceDetailModel);
                            code = ((int)ReturnCodeEnum.READ_SUCCESS).ToString();
                            status = true;
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int)AccessEnum.ADMINISTRATOR_ACCESS ? ex.Message : ((int)ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    break;
                case "update":
                    if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.UPDATE_ACCESS))
                    {
                        code = ((int)ReturnCodeEnum.ACCESS_DENIED).ToString();
                    }
                    else
                    {
                        try
                        {
                            InvoiceDetailModel invoiceDetailModel = new()
                            {
			InvoiceDetailKey = invoiceDetailKey,
			InvoiceKey = invoiceKey,
			ProductKey = productKey,
			InvoiceDetailUnitPrice = invoiceDetailUnitPrice,
			InvoiceDetailQuantity = invoiceDetailQuantity,
			InvoiceDetailDiscount = invoiceDetailDiscount,
                            };
                            invoiceDetailRepository.Update(invoiceDetailModel);
                            code = ((int)ReturnCodeEnum.UPDATE_SUCCESS).ToString();
                            status = true;
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int)AccessEnum.ADMINISTRATOR_ACCESS ? ex.Message : ((int)ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    break;
                case "delete":
                    if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.DELETE_ACCESS))
                    {
                        code = ((int)ReturnCodeEnum.ACCESS_DENIED).ToString();
                    }
                    else
                    {
                        try
                        {
                            InvoiceDetailModel invoiceDetailModel = new()
                            {
                                InvoiceDetailKey = invoiceDetailKey
                            };
                            invoiceDetailRepository.Delete(invoiceDetailModel);
                            code = ((int)ReturnCodeEnum.DELETE_SUCCESS).ToString();
                            status = true;
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int)AccessEnum.ADMINISTRATOR_ACCESS ? ex.Message : ((int)ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    break;
                default:
                    code = ((int)ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                    break;
            }
            if (data.Count > 0)
            {
                return Ok(new { status, code, data });
            }
            if (mode.Equals("single"))
            {
                return Ok(new { status, code, dataSingle });
            }
            return lastInsertKey > 0 ? Ok(new { status, code, lastInsertKey }) : Ok(new { status, code });
        }
     
    }
