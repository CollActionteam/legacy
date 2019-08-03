using CollAction.Data;
using CollAction.Models;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace CollAction.Services.DataProtection
{
    public sealed class DataProtectionRepository : IXmlRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> options;

        // Unfortunuatly we can't dependency-inject the context safely here, due to the way dataprotection repositories are configured (see startup)
        // So, we're injecting the dbcontext options, and initialize the context ourselves
        public DataProtectionRepository(DbContextOptions<ApplicationDbContext> options)
        {
            this.options = options;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            using (var context = new ApplicationDbContext(options))
            {
                return context.DataProtectionKeys
                              .Select(key => XElement.Parse(key.KeyDataXml))
                              .ToArray();
            }
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            using (var context = new ApplicationDbContext(options))
            {
                context.DataProtectionKeys.Add(new DataProtectionKey()
                {
                    FriendlyName = friendlyName,
                    KeyDataXml = element.ToString(SaveOptions.DisableFormatting)
                });

                context.SaveChanges();
            }
        }
    }
}