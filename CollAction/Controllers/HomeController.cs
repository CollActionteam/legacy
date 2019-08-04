using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;
using CollAction.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CollAction.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment, ILogger<HomeController> logger)
        {
            this.logger = logger;
            this.hostingEnvironment = hostingEnvironment;
        }

        [Route("error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError(exceptionHandlerPathFeature.Error, "An error has occurred at: {0}", exceptionHandlerPathFeature.Path);
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            if (hostingEnvironment.IsDevelopment())
            {
                return Json(new { error = $"An internal error has occured: {exceptionHandlerPathFeature.Error.GetExceptionDetails()}" });
            }
            else
            {
                return Json(new { error = "An internal error has occured" });
            }
        }
    }
}
