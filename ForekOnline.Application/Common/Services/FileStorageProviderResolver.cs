using ForekOnline.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Services
{
    public class FileStorageProviderResolver : IFileStorageProviderResolver
    {
        private readonly IReadOnlyDictionary<string, IFileStorageProvider> _providersByName;
        private readonly IFileStorageProvider _defaultProvider;

        public FileStorageProviderResolver(IEnumerable<IFileStorageProvider> providers)
        {
            if (providers is null) throw new ArgumentNullException(nameof(providers));

            var providerList = providers.ToList();

            if (providerList.Count == 0)
            {
                throw new InvalidOperationException("No IFileStorageProvider implementations were registered.");
            }

            _providersByName = providerList
                .Where(p => !string.IsNullOrWhiteSpace(p.ProviderName))
                .ToDictionary(p => p.ProviderName, p => p, StringComparer.OrdinalIgnoreCase);

            _defaultProvider = providerList[0];
        }

        public IFileStorageProvider Resolve(string? providerHint)
        {
            if (!string.IsNullOrWhiteSpace(providerHint) && _providersByName.TryGetValue(providerHint, out var provider))
            {
                return provider;
            }

            return _defaultProvider;
        }
    }
}
