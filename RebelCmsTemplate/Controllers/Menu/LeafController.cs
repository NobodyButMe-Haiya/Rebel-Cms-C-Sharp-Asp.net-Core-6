﻿using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Menu;
using RebelCmsTemplate.Repository.Menu;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Api.Menu;

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

        LeafRepository leafRepository = new(_httpContextAccessor);
        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);

        List<LeafModel> data = new();
        var lastInsertKey = 0;

        string? code = string.Empty;
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
                        int folderKey = 0;
                        if (!int.TryParse(Request.Form["folderKey"], out folderKey))
                        {
                            code = ((int)ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                            return Ok(new { status, code });
                        }
                        var leafSeq = Convert.ToInt32(Request.Form["leafSeq"]);

                        var leafName = Request.Form["leafName"];
                        var leafFilename = Request.Form["leafFilename"];
                        var leafIcon = Request.Form["leafIcon"];



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
                        return Ok(new { status, code, lastInsertKey });
                    }
                    catch (Exception ex)
                    {
                        code = sharedUtil.GetRoleId() == (int)AccessEnum.ADMINISTRATOR_ACCESS
                            ? ex.Message
                            : ((int)ReturnCodeEnum.SYSTEM_ERROR).ToString();
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
                        return Ok(new { status, code, data });
                    }
                    catch (Exception ex)
                    {
                        code = sharedUtil.GetRoleId() == (int)AccessEnum.ADMINISTRATOR_ACCESS
                            ? ex.Message
                            : ((int)ReturnCodeEnum.SYSTEM_ERROR).ToString();
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
                    if (!string.IsNullOrEmpty(Request.Form["search"]))
                    {
                        try
                        {
                            var search = Request.Form["search"];
                            data = leafRepository.Search(search);
                            code = ((int)ReturnCodeEnum.READ_SUCCESS).ToString();
                            status = true;
                            return Ok(new { status, code, data });
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int)AccessEnum.ADMINISTRATOR_ACCESS
                                ? ex.Message
                                : ((int)ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    else
                    {
                        code = ((int)ReturnCodeEnum.ACCESS_DENIED).ToString();
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
                    if (!string.IsNullOrEmpty(Request.Form["leafId"]))
                    {

                        try
                        {
                            int leafKey = 0;
                            if (!int.TryParse(Request.Form["leafKey"], out leafKey))
                            {
                                code = ((int)ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new { status, code });
                            }
                            if (leafKey > 0)
                            {
                                int folderKey = 0;
                                if (!int.TryParse(Request.Form["folderKey"], out folderKey))
                                {
                                    code = ((int)ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                    return Ok(new { status, code });
                                }
                                var leafSeq = Convert.ToInt32(Request.Form["leafSeq"]);
                                var leafName = Request.Form["leafName"];
                                var leafFilename = Request.Form["leafFilename"];
                                var leafIcon = Request.Form["leafIcon"];

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
                            else
                            {
                                code = ((int)ReturnCodeEnum.ACCESS_DENIED).ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int)AccessEnum.ADMINISTRATOR_ACCESS
                                ? ex.Message
                                : ((int)ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    else
                    {
                        code = ((int)ReturnCodeEnum.ACCESS_DENIED).ToString();
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

                    if (!string.IsNullOrEmpty(Request.Form["leafId"]))
                    {

                        try
                        {
                            int leafKey = 0;
                            if (!int.TryParse(Request.Form["leafKey"], out leafKey))
                            {
                                code = ((int)ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new { status, code });
                            }
                            if (leafKey > 0)
                            {
                               
                                LeafModel leafModel = new()
                                {
                                    LeafKey = leafKey
                                };
                                leafRepository.Delete(leafModel);

                                code = ((int)ReturnCodeEnum.DELETE_SUCCESS).ToString();
                                status = true;
                            }
                            else
                            {
                                code = ((int)ReturnCodeEnum.ACCESS_DENIED).ToString();

                            }
                        }
                        catch (Exception ex)
                        {
                            code = sharedUtil.GetRoleId() == (int)AccessEnum.ADMINISTRATOR_ACCESS
                                ? ex.Message
                                : ((int)ReturnCodeEnum.SYSTEM_ERROR).ToString();
                        }
                    }
                    else
                    {
                        code = ((int)ReturnCodeEnum.ACCESS_DENIED).ToString();

                    }
                }

                break;
            default:
                code = ((int)ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                break;
        }

        return Ok(new { status, code });
    }
}