using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CollAction.Services.ViewRender
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IUrlHelper _urlHelper;

        public ViewRenderService(ICompositeViewEngine viewEngine, IUrlHelper urlHelper)
        {
            _viewEngine = viewEngine;
            _urlHelper = urlHelper;
        }

        public Task<string> Render(string viewPath)
            => Render(viewPath, string.Empty);

        public async Task<string> Render<TModel>(string viewPath, TModel model)
        {
            ViewEngineResult viewEngineResult = _viewEngine.GetView("~/", viewPath, false);

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException($"Couldn't find view {viewPath}");
            }

            IView view = viewEngineResult.View;

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext()
                {
                    HttpContext = _urlHelper.ActionContext.HttpContext,
                    RouteData = _urlHelper.ActionContext.RouteData,
                    ActionDescriptor = _urlHelper.ActionContext.ActionDescriptor,
                    View = view,
                    ViewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    },
                    Writer = output
                };

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }
    }
} 