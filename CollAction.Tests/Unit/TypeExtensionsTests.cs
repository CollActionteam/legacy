using CollAction.GraphQl.Queries;
using CollAction.Helpers;
using CollAction.Models;
using GraphQL.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CollAction.Tests.Unit
{
    [TestClass]
    public sealed class TypeExtensionsTests
    {
        [TestMethod]
        public void TestGetGenericBaseClass()
        {
            Assert.AreEqual(typeof(EnumerationGraphType<Category>), typeof(CategoryGraph).GetGenericBaseClass(typeof(EnumerationGraphType<>)));
        }

        [TestMethod]
        public void TestIsAssignableToGenericType()
        {
            Assert.IsTrue(typeof(CategoryGraph).IsAssignableToGenericType(typeof(EnumerationGraphType<>)));
            Assert.IsTrue(typeof(List<int>).IsAssignableToGenericType(typeof(List<>)));
            Assert.IsFalse(typeof(IEnumerable<int>).IsAssignableToGenericType(typeof(List<>)));
            Assert.IsTrue(typeof(List<int>).IsAssignableToGenericType(typeof(IEnumerable<>)));
        }
    }
}
