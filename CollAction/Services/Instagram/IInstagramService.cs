using CollAction.Services.Instagram.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CollAction.Services.Instagram
{
    public interface IInstagramService
    {
        public Task<IEnumerable<InstagramTimelineItem>> GetItems(string instagramName, CancellationToken token);
    }
}
