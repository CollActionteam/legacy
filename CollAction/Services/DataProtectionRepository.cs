using CollAction.Data;
using CollAction.Models;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace CollAction.Services
{
    public sealed class DataProtectionRepository : IXmlRepository
    {
        private readonly IServiceProvider _provider;

        public DataProtectionRepository(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
            => _provider.GetService<ApplicationDbContext>()
                        .DataProtectionKeys
                        .Select(key => XElement.Parse(key.KeyDataXml))
                        .ToArray();

        public void StoreElement(XElement element, string friendlyName)
        {
            ApplicationDbContext context = _provider.GetService<ApplicationDbContext>();
            context.DataProtectionKeys.Add(new DataProtectionKey()
            {
                FriendlyName = friendlyName,
                KeyDataXml = element.ToString(SaveOptions.DisableFormatting)
            });

            context.SaveChanges();
        }
    }
}
