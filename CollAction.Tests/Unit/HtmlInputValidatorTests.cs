using CollAction.Services.HtmlValidator;
using Xunit;

namespace CollAction.Tests.Unit
{
    [Trait("Category", "Unit")]
    public sealed class HtmlInputValidatorTests
    {
        [Fact]
        public void TestHtmlInputValidation()
        {
            var validator = new HtmlInputValidator();
            Assert.True(validator.IsSafe("<p>test</p>"));
            Assert.True(validator.IsSafe("<a href=\"http://www.google.com\">test</a>"));
            Assert.False(validator.IsSafe("<a href=\"ftp://evil.com\">test</a>"));
            Assert.False(validator.IsSafe("<a href=\"javascript:doSomething()\">test</a>"));
            Assert.False(validator.IsSafe("<script src=\"https://evil.com\">test</script>"));
        }
    }
}
