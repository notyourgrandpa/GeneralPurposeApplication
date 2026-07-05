using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Services
{
    public interface IUserLookupService
    {
        Task<Dictionary<string, string>> GetUserNamesAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}
