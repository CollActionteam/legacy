namespace CollAction.Helpers
{
    public interface IHtmlInputValidator
    {
        bool IsSafe(string inputHtml);
    }
}