using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Menu;
using RebelCmsTemplate.Repository.Menu;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Api.Menu
{
    [Route("api/menu/[controller]")]
    [ApiController]
    public class LeafController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RenderViewToStringUtil _renderViewToStringUtil;

        public LeafController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
        {
            _renderViewToStringUtil = renderViewToStringUtil;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            const string? templatePath = "~/Views/Error/403.cshtml";
            var page = await _renderViewToStringUtil.RenderViewToStringAsync(ControllerContext, templatePath);
            return Ok(page);
        }
        [HttpPost]
        public ActionResult Post()
        {
            var status = false;
            var mode = Request.Form["mode"];
            var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);

            var leafKey = Convert.ToInt32(Request.Form["leafKey"]);
            var folderKey = Convert.ToInt32(Request.Form["folderKey"]);
            var leafSeq = Convert.ToInt32(Request.Form["leafSeq"]);

            var leafName = Request.Form["leafName"];
            var leafFilename = Request.Form["leafFilename"];
            var leafIcon = Request.Form["leafIcon"];

            var search = Request.Form["search"];

            LeafRepository leafRepository = new(_httpContextAccessor);
            SharedUtil sharedUtil = new(_httpContextAccessor);
            CheckAccessUtil checkAccessUtil = new (_httpContextAccessor);

            List<LeafModel> data = new();
            var lastInsertKey = 0;

            string? code;
            // but we think something missing .. what ya ? 
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
                            LeafModel leafModel = new()
                            {
                                FolderKey = folderKey,
                                LeafName = leafName,
                                LeafFilename = leafFilename,
                                LeafIcon = leafIcon,
                                LeafSeq = leafSeq
                            };
                            lastInsertKey = leafRepository.Create(leafModel);
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
                            data = leafRepository.Read();
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
                            data = leafRepository.Search(search);
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
                            LeafModel leafModel = new()
                            {
                                FolderKey = folderKey,
                                LeafName = leafName,
                                LeafFilename = leafFilename,
                                LeafIcon = leafIcon,
                                LeafSeq = leafSeq,
                                LeafKey = leafKey
                            };
                            leafRepository.Update(leafModel);
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
                            LeafModel leafModel = new()
                            {
                                LeafKey = leafKey
                            };
                            leafRepository.Delete(leafModel);

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
