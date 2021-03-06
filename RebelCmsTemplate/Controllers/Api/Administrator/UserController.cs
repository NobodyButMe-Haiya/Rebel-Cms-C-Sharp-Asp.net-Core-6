using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Models.Administrator;
using RebelCmsTemplate.Repository.Administrator;
using RebelCmsTemplate.Util;
using RebelCmsTemplate.Enum;

namespace RebelCmsTemplate.Controllers.Api.Administrator
{
    [Route("api/administrator/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RenderViewToStringUtil _renderViewToStringUtil;

        public UserController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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
            UserRepository userRepository = new(_httpContextAccessor);
            var content = userRepository.GetExcel();
            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "roles.xlsx");
        }

        [HttpPost]
        public ActionResult Post()
        {
            var status = false;
            string? code;
            var mode = Request.Form["mode"];
            var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);


            var userKey = Convert.ToInt32(Request.Form["userKey"]);
            
            var roleKey = 0;
            if (!string.IsNullOrWhiteSpace(Request.Form["roleKey"]))
            {
                var test = int.TryParse(Request.Form["roleKey"], out roleKey);
                if (!test)
                {
                    code = ((int)ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                    return Ok(new {status,code});
                }
            }
            var userName = Request.Form["userName"].ToString();
            var userPassword = Request.Form["userPassword"].ToString();
            var userEmail = Request.Form["userEmail"].ToString();
            var userPhone = Request.Form["userPhone"].ToString();
            var userAddress = Request.Form["userAddress"].ToString();

            var search = Request.Form["search"];

            UserRepository userRepository = new(_httpContextAccessor);
            SharedUtil sharedUtil = new(_httpContextAccessor);
            CheckAccessUtil checkAccessUtil = new (_httpContextAccessor);

            List<UserModel> data = new();
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
                            lastInsertKey = userRepository.Create(new UserModel
                            {
                                RoleKey = roleKey,
                                UserName = userName,
                                UserPassword = userPassword,
                                UserEmail = userEmail,
                                UserPhone = userPhone,
                                UserAddress =userAddress
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
                            data = userRepository.Read();
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
                            data = userRepository.Search(search);
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
                            userRepository.Update(new UserModel
                            {
                                RoleKey = roleKey,
                                UserName = userName,
                                UserPassword = userPassword,
                                UserEmail = userEmail,
                                UserPhone = userPhone,
                                UserAddress =userAddress,
                                UserKey = userKey
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
                            userRepository.Delete(new UserModel
                            {
                                UserKey = userKey
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
                    code = SharedUtil.Return500();
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
