using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CollAction.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        [Route("error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError(exceptionHandlerPathFeature.Error, "An error has occurred at: {0}", exceptionHandlerPathFeature.Path);
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return Json(new { error = "An internal error has occured" });
        }
    }
}
