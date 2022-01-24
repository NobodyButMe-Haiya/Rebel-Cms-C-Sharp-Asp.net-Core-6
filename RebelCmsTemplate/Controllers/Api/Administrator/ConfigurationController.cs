using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Enum;
using RebelCmsTemplate.Models.Administrator;
using RebelCmsTemplate.Repository.Administrator;
using RebelCmsTemplate.Util;

/***
 * @link https://creativecommons.org/licenses/by-nc/4.0/
 */
namespace RebelCmsTemplate.Controllers.Api.Administrator;

[Route("api/administrator/[controller]")]
[ApiController]
public class ConfigurationController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public ConfigurationController(RenderViewToStringUtil renderViewToStringUtil,
        IHttpContextAccessor httpContextAccessor)
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
        SharedUtil sharedUtil = new(_httpContextAccessor);
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);
        var status = false;
        var mode = Request.Form["mode"];
        var leafCheckKey = Convert.ToInt32(Request.Form["leafCheckKey"]);

        var configurationKey = Convert.ToInt32(Request.Form["configurationKey"]);
        var configurationPortal = Request.Form["configurationPortal"].ToString();
        var configurationPortalLocal = Request.Form["configurationPortalLocal"].ToString();
        var configurationEmail = Request.Form["configurationEmail"].ToString();
        var configurationEmailHost = Request.Form["configurationEmailHost"].ToString();
        var configurationEmailPassword = Request.Form["configurationEmailPassword"].ToString();
        var configurationEmailPort = Request.Form["configurationEmailPort"].ToString();
        var configurationEmailSecure = Request.Form["configurationEmailSecure"].ToString();


        ConfigurationRepository configurationRepository = new(_httpContextAccessor);
        List<ConfigurationModel> data = new();

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
                        data = configurationRepository.Read();
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
            case "update":
                if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.UPDATE_ACCESS))
                {
                    code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                }
                else
                {
                    try
                    {
                        ConfigurationModel configurationModel = new()
                        {
                            ConfigurationKey = configurationKey,
                            ConfigurationPortal = configurationPortal,
                            ConfigurationPortalLocal = configurationPortalLocal,
                            ConfigurationEmail = configurationEmail,
                            ConfigurationEmailHost = configurationEmailHost,
                            ConfigurationEmailPassword = configurationEmailPassword,
                            ConfigurationEmailPort = configurationEmailPort,
                            ConfigurationEmailSecure = configurationEmailSecure
                        };
                        configurationRepository.Update(configurationModel);
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
            default:
                code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                break;
        }

        return data.Count > 0 ? Ok(new {status, code, data}) : Ok(new {status, code});
    }
}