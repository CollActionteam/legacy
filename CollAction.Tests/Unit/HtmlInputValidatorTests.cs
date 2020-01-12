using CollAction.Services.HtmlValidator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CollAction.Tests.Unit
{
    [TestClass]
    public class HtmlInputValidatorTests
    {
        [TestMethod]
        public void TestHtmlInputValidation()
        {
            var validator = new HtmlInputValidator();
            Assert.IsTrue(validator.IsSafe("<p>test</p>"));
            Assert.IsTrue(validator.IsSafe("<a href=\"http://www.google.com\">test</a>"));
            Assert.IsFalse(validator.IsSafe("<a href=\"ftp://evil.com\">test</a>"));
            Assert.IsFalse(validator.IsSafe("<a href=\"javascript:doSomething()\">test</a>"));
            Assert.IsFalse(validator.IsSafe("<script src=\"https://evil.com\">test</script>"));
        }
    }
}
