using CollAction.Services.Instagram;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CollAction.Tests.Integration.Service
{
    [Trait("Category", "Integration")]
    [Trait("SkipInActions", "true")]
    public sealed class InstagramServiceTests : IntegrationTestBase
    {
        private readonly IInstagramService instagramService;

        public InstagramServiceTests(): base(false)
        {
            instagramService = Scope.ServiceProvider.GetRequiredService<IInstagramService>();
        }

        [Fact]
        public async Task TestInstagramApi()
        {
            var result = await instagramService.GetItems("slowfashionseason", CancellationToken.None).ConfigureAwait(false);
            Assert.True(result.Any());
        }
    }
}
