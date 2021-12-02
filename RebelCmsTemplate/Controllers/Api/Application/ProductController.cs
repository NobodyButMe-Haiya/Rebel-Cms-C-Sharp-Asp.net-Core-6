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

        // GET: /<controller>/
        [Route("api/administrator/[controller]")]
        [ApiController]
        public class ProductController : Controller
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly RenderViewToStringUtil _renderViewToStringUtil;

            public ProductController(RenderViewToStringUtil renderViewToStringUtil,
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
            ProductRepository productRepository = new(_httpContextAccessor);
            var content = productRepository.GetExcel();
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


                var productKey = Convert.ToInt32(Request.Form["productKey"]);

                var productCategoryKey = 0;
                if (!string.IsNullOrWhiteSpace(Request.Form["productCategoryKey"]))
                {
                    var test = int.TryParse(Request.Form["productCategoryKey"], out productCategoryKey);
                    if (!test)
                    {
                        code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                        return Ok(new {status, code});
                    }
                }

                var productTypeKey = 0;
                if (!string.IsNullOrWhiteSpace(Request.Form["productTypeKey"]))
                {
                    var test = int.TryParse(Request.Form["productTypeKey"], out productTypeKey);
                    if (!test)
                    {
                        code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                        return Ok(new {status, code});
                    }
                }
                var productName = Request.Form["productName"].ToString();
                var productDescription = Request.Form["productDescription"].ToString();
                var productCostPrice = 0.0;
                if (!string.IsNullOrWhiteSpace(Request.Form["productCostPrice"]))
                {
                    var test = double.TryParse(Request.Form["productCostPrice"], out productCostPrice);
                    if (!test)
                    {
                        code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                        return Ok(new {status, code});
                    }
                }
                var productSellingPrice = 0.0;
                if (!string.IsNullOrWhiteSpace(Request.Form["productSellingPrice"]))
                {
                    var test = double.TryParse(Request.Form["productSellingPrice"], out productSellingPrice);
                    if (!test)
                    {
                        code = ((int) ReturnCodeEnum.ACCESS_DENIED_NO_MODE).ToString();
                        return Ok(new {status, code});
                    }
                }

                var search = Request.Form["search"];

                ProductRepository productRepository = new(_httpContextAccessor);
                SharedUtil sharedUtil = new(_httpContextAccessor);
                CheckAccessUtil checkAccessUtil = new(_httpContextAccessor);

                List<ProductModel> data = new();
                var lastInsertKey = 0;

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
                                lastInsertKey = productRepository.Create(new ProductModel
                                {
                                    ProductCategoryKey = productCategoryKey,
                                    ProductTypeKey = productTypeKey,
                                    ProductName = productName,
                                    ProductDescription = productDescription,
                                    ProductCostPrice = productCostPrice,
                                    ProductSellingPrice = productSellingPrice
                                });
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
                    case "read":
                        if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.READ_ACCESS))
                        {
                            code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                        }
                        else
                        {
                            try
                            {
                                data = productRepository.Read();
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
                                data = productRepository.Search(search);
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
                            try
                            {
                                productRepository.Update(new ProductModel
                                {
                                    ProductCategoryKey = productCategoryKey,
                                    ProductTypeKey = productTypeKey,
                                    ProductName = productName,
                                    ProductDescription = productDescription,
                                    ProductCostPrice = productCostPrice,
                                    ProductSellingPrice = productSellingPrice,
                                    ProductKey = productKey
                                });
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
                    case "delete":
                        if (!checkAccessUtil.GetPermission(leafCheckKey, AuthenticationEnum.DELETE_ACCESS))
                        {
                            code = ((int) ReturnCodeEnum.ACCESS_DENIED).ToString();
                        }
                        else
                        {
                            try
                            {
                                productRepository.Delete(new ProductModel
                                {
                                    ProductKey = productKey
                                });

                                code = ((int) ReturnCodeEnum.DELETE_SUCCESS).ToString();
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
                        code = SharedUtil.Return500();
                        break;
                }

                if (data.Count > 0)
                {
                    return Ok(new {status, code, data});
                }

                return lastInsertKey > 0 ? Ok(new {status, code, lastInsertKey}) : Ok(new {status, code});
            }
        }
    }