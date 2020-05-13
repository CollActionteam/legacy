using CollAction.GraphQl.Queries;
using CollAction.Helpers;
using CollAction.Models;
using GraphQL.Types;
using System.Collections.Generic;
using Xunit;

namespace CollAction.Tests.Unit
{
    [Trait("Category", "Unit")]
    public sealed class TypeExtensionsTests
    {
        [Fact]
        public void TestGetGenericBaseClass()
        {
            Assert.Equal(typeof(EnumerationGraphType<Category>), typeof(CategoryGraph).GetGenericBaseClass(typeof(EnumerationGraphType<>)));
        }

        [Fact]
        public void TestIsAssignableToGenericType()
        {
            Assert.True(typeof(CategoryGraph).IsAssignableToGenericType(typeof(EnumerationGraphType<>)));
            Assert.True(typeof(List<int>).IsAssignableToGenericType(typeof(List<>)));
            Assert.False(typeof(IEnumerable<int>).IsAssignableToGenericType(typeof(List<>)));
            Assert.True(typeof(List<int>).IsAssignableToGenericType(typeof(IEnumerable<>)));
        }
    }
}
