using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Setting;
using RebelCmsTemplate.Repository.Setting;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Api.Setting
{
    [Route("api/setting/[controller]")]
    [ApiController]
    public class ProductCategoryController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RenderViewToStringUtil _renderViewToStringUtil;

        public ProductCategoryController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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
            ProductCategoryRepository productCategoryRepository = new(_httpContextAccessor);
            var content = productCategoryRepository.GetExcel();
            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "roles.xlsx");
        }
        [HttpPost]
        public ActionResult Post()
        {
            var status = false;
            var mode = Request.Form["mode"];
            var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);

            var productCategoryName = Request.Form["productCategoryName"];
            var productCategoryKey = Convert.ToInt32(Request.Form["productCategoryKey"]);
            var search = Request.Form["search"];

            ProductCategoryRepository productCategoryRepository = new(_httpContextAccessor);
            SharedUtil sharedUtil = new(_httpContextAccessor);
            CheckAccessUtil checkAccessUtil = new (_httpContextAccessor);

            List<ProductCategoryModel> data = new();
            var lastInsertKey = 0;

            string? code;
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
                            lastInsertKey = productCategoryRepository.Create(new ProductCategoryModel
                            {
                                ProductCategoryName = productCategoryName
                            });
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
                            data = productCategoryRepository.Read();
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
                            data = productCategoryRepository.Search(search);
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
                            productCategoryRepository.Update(new ProductCategoryModel
                            {
                                ProductCategoryName = productCategoryName,
                                ProductCategoryKey = productCategoryKey
                            });
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
                            productCategoryRepository.Delete(new ProductCategoryModel
                            {
                                ProductCategoryKey = productCategoryKey
                            });

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

            return lastInsertKey > 0 ? Ok(new { status, code, lastInsertKey }) : Ok(new { status, code });
        }
    }

}
