using NServiceBus;
using NServiceBus.Encryption.MessageProperty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Messages.Shared
{
    public static class EncryptionExtensions
    {
        #region ConfigureEncryption

        public static void ConfigurationEncryption(this EndpointConfiguration endpointConfiguration)
        {
            var defaultKey = "2015-10";

            var keys = new Dictionary<string, byte[]>
            {
                {"2015-10", Convert.FromBase64String("gdDbqRpqdRbTs3mhdZh9qCaDaxJXl+e6")},
                {"2015-09", Convert.FromBase64String("abDbqRpQdRbTs3mhdZh9qCaDaxJXl+e6")},
                {"2015-08", Convert.FromBase64String("cdDbqRpQdRbTs3mhdZh9qCaDaxJXl+e6")},
            };

            var encryptionService = new RijndaelEncryptionService(defaultKey, keys);
            endpointConfiguration.EnableMessagePropertyEncryption(encryptionService,
                encryptedPropertyConvention: info =>
                {
                    return info.Name.StartsWith("Encrypted");
                });
        }

        #endregion
    }
}
