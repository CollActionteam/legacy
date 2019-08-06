namespace CollAction.Services.HtmlValidator
{
    public interface IHtmlInputValidator
    {
        bool IsSafe(string inputHtml);
    }
}