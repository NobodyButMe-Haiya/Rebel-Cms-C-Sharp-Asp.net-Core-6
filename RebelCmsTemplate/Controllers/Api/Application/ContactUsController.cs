using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Repository.Application;
using RebelCmsTemplate.Util;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RebelCmsTemplate.Controllers.Api.Application
{
    [Route("api/application/[controller]")]
    [ApiController]
    public class ContactUsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RenderViewToStringUtil _renderViewToStringUtil;
        public ContactUsController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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
            ContactUsRepository contactUsRepository = new(_httpContextAccessor);
            var content = contactUsRepository.GetExcel();
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
            var contactUsId = Convert.ToInt32(Request.Form["contactUsId"]);

            var search = Request.Form["search"];

            ContactUsRepository contactUsRepository = new(_httpContextAccessor);
            SharedUtil sharedUtil = new(_httpContextAccessor);
            CheckAccessUtil checkAccessUtil = new (_httpContextAccessor);

            List<ContactUsModel> data = new();

            string? code;
            switch (mode)
            {
                case "read":
                    if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                    {
                        code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                    }
                    else
                    {
                        try
                        {
                            data = contactUsRepository.Read();
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
                            data = contactUsRepository.Search(search);
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
                        ContactUsModel contactUsModel = new()
                        {
                            ContactUsKey = contactUsId
                        };
                        try
                        {
                            contactUsRepository.Update(contactUsModel);
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
                case "delete":

                    if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.DELETE_ACCESS))
                    {
                        code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                    }
                    else
                    {
                        ContactUsModel contactUsModel = new()
                        {
                            ContactUsKey = contactUsId
                        };
                        try
                        {
                            contactUsRepository.Delete(contactUsModel);
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
                default:
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                    break;
            }

            return data.Count > 0 ? Ok(new {status, code, data}) : Ok(new {status, code});
        }
    }
}