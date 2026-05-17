using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IStoredDocumentLookup
    {
        Task<string?> GetProviderNameAsync(Guid documentId, CancellationToken cancellationToken = default);
    }
}
