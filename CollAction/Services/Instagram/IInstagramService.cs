using CollAction.Services.Instagram.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Instagram
{
    public interface IInstagramService
    {
        public Task<IEnumerable<InstagramWallItem>> GetItems(string instagramUser, CancellationToken token);
    }
}
