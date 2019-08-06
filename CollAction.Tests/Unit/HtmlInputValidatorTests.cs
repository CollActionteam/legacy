using CollAction.Helpers;
using CollAction.Services.HtmlValidator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CollAction.Tests.Unit
{
    [TestClass]
    public class HtmlValidatorTests
    {
        [TestMethod]
        public void TestDangerousInput()
        {
            var validator = new HtmlInputValidator();
            Assert.IsTrue(validator.IsSafe("<p>test</p>"));
            Assert.IsTrue(validator.IsSafe("<a href=\"https://google.com\">test</a>"));
            Assert.IsFalse(validator.IsSafe("<a style=\"display: none\" href=\"https://google.com\">test</a>"));
            Assert.IsFalse(validator.IsSafe("<script>test</script>"));
            Assert.IsFalse(validator.IsSafe("<div style=\"background-image: url(javascript: alert('Injected'))\">"));
            Assert.IsFalse(validator.IsSafe("<img src=\"x\" onerror=\"alert(9)\">"));
            Assert.IsFalse(validator.IsSafe("<img src=\"javascript:alert(9)\">"));
            Assert.IsFalse(validator.IsSafe("<a href=\"javascript:alert(9)\">test</a>"));
        }
    }
}
