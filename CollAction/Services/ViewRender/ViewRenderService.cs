using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CollAction.Services.ViewRender
{
    public sealed class ViewRenderService : IViewRenderService
    {
        private readonly ICompositeViewEngine viewEngine;
        private readonly IUrlHelper urlHelper;

        public ViewRenderService(ICompositeViewEngine viewEngine, IUrlHelper urlHelper)
        {
            this.viewEngine = viewEngine;
            this.urlHelper = urlHelper;
        }

        public Task<string> Render(string viewPath)
            => Render(viewPath, string.Empty);

        public async Task<string> Render<TModel>(string viewPath, TModel model)
        {
            ViewEngineResult viewEngineResult = viewEngine.GetView("~/", viewPath, false);

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException($"Couldn't find view {viewPath}");
            }

            IView view = viewEngineResult.View;

            using var output = new StringWriter();
            var viewContext = new ViewContext()
            {
                HttpContext = urlHelper.ActionContext.HttpContext,
                RouteData = urlHelper.ActionContext.RouteData,
                ActionDescriptor = urlHelper.ActionContext.ActionDescriptor,
                View = view,
                ViewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                Writer = output
            };

            await view.RenderAsync(viewContext).ConfigureAwait(false);

            return output.ToString();
        }
    }
} 