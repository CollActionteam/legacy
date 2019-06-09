using System.Threading.Tasks;

namespace CollAction.Services.ViewRender
{
    public interface IViewRenderService
    {
        Task<string> Render(string viewPath);
        Task<string> Render<TModel>(string viewPath, TModel model);
    }
}