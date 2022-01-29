using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers.Web;

[Route("42/[controller]")]
[ApiController]
public class WebController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RenderViewToStringUtil _renderViewToStringUtil;

    public WebController(RenderViewToStringUtil renderViewToStringUtil, IHttpContextAccessor httpContextAccessor)
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
        CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);
        var mode = Request.Form["mode"];
        var userName = Request.Form["beauty"];
        var userPassword = Request.Form["beast"];
        var token = Request.Form["disney"];
        bool status;
        if (mode == "web")
        {
            status = checkAccessUtil.GetCheckAccessFromWeb(userName, userPassword);
            return Ok(new {status});
        }

        status = checkAccessUtil.GetCheckAccessFromMobile(token);
        return Ok(new {status, token = checkAccessUtil.Token});
    }
}