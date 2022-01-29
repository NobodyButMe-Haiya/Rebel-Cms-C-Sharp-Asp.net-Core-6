using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Menu;
using RebelCmsTemplate.Repository.Menu;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Api.Menu;

[Route("api/menu/[controller]")]
[ApiController]
public class FolderController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public FolderController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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





        FolderRepository folderRepository = new(_httpContextAccessor);
        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);

        List<FolderModel> data = new();
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
                        var folderKey = Convert.ToInt32(Request.Form["folderKey"]);
                        var folderName = Request.Form["folderName"];
                        var folderFilename = Request.Form["folderFilename"];
                        var folderIcon = Request.Form["folderIcon"];
                        var folderSeq = Convert.ToInt32(Request.Form["folderSeq"]);

                        FolderModel folderModel = new()
                        {
                            FolderName = folderName,
                            FolderFilename = folderFilename,
                            FolderIcon = folderIcon,
                            FolderSeq = folderSeq
                        };
                        lastInsertKey = folderRepository.Create(folderModel);
                        code = ((int)ReturnCodeEnum.CREATE_SUCCESS).ToString();
                        status = true;
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
                        data = folderRepository.Read();
                        code = ((int)ReturnCodeEnum.CREATE_SUCCESS).ToString();
                        status = true;
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
                            data = folderRepository.Search(search);
                            code = ((int)ReturnCodeEnum.READ_SUCCESS).ToString();
                            status = true;
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
                    if (!string.IsNullOrEmpty(Request.Form["folderKey"]))
                    {
                        try
                        {
                            int folderKey = 0;
                            if (!int.TryParse(Request.Form["folderKey"], out folderKey))
                            {
                                code = ((int)ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new { status, code });
                            }

                            var folderKey = Convert.ToInt32(Request.Form["folderKey"]);
                            var folderName = Request.Form["folderName"];
                            var folderFilename = Request.Form["folderFilename"];
                            var folderIcon = Request.Form["folderIcon"];
                            var folderSeq = Convert.ToInt32(Request.Form["folderSeq"]);
                            if (folderKey > 0)
                            {
                                FolderModel folderModel = new()
                                {
                                    FolderName = folderName,
                                    FolderFilename = folderFilename,
                                    FolderIcon = folderIcon,
                                    FolderSeq = folderSeq,
                                    FolderKey = folderKey
                                };
                                folderRepository.Update(folderModel);
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
                    if (!string.IsNullOrEmpty(Request.Form["folderKey"]))
                    {
                        try
                        {
                            int folderKey = 0;
                            if (!int.TryParse(Request.Form["folderKey"], out folderKey))
                            {
                                code = ((int)ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                                return Ok(new { status, code });
                            }
                            if (folderKey > 0)
                            {
                                FolderModel folderModel = new()
                                {
                                    FolderKey = Convert.ToInt32(folderKey)
                                };
                                folderRepository.Delete(folderModel);

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

        if (data.Count > 0)
        {
            return Ok(new { status, code, data });
        }

        return lastInsertKey > 0 ? Ok(new { status, code, lastInsertKey }) : Ok(new { status, code });
    }
}