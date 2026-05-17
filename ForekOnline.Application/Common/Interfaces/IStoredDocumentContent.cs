using ForekOnline.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Interfaces
{
    public interface IStoredDocumentContent : IRepository<StoredDocumentContent>
    {
        Task<StoredDocumentContent> UpdateStoredDocumentContentAsync(StoredDocumentContent storedDocumentContent);
    }
}
