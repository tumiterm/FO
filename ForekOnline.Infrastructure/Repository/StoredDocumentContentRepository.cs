using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Domain.Entities;
using ForekOnline.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Infrastructure.Repository
{
    public class StoredDocumentContentRepository : Repository<StoredDocumentContent>, IStoredDocumentContent
    {
        private readonly ApplicationDbContext _context;

        public StoredDocumentContentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<StoredDocumentContent> UpdateStoredDocumentContentAsync(StoredDocumentContent storedDocumentContent)
        {
            _context.StoredDocumentContents.Update(storedDocumentContent);

            await _context.SaveChangesAsync();

            return storedDocumentContent;
        }
    }
}
