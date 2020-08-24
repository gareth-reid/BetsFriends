using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using BetfairNG;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace BF_API
{
    public class ApiConfig
    {
        public BetfairClient BetfairClient;
        public string ThumbPrint;
        public ApiConfig(ExecutionContext context)
        {
#if DEBUG
            BetfairClient = new BetfairClient("UhwmsL3EqCwjEKwH");
            BetfairClient.Login(@"/etc/client-2048.p12", "REDsky.123", "garethreid123@gmail.com", "REDsky.123", null);
#else
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true) // <- This gives you access to your application settings in your local development environment
                .AddEnvironmentVariables() // <- This is what actually gets you the application settings in Azure
                .Build();

            ThumbPrint = config["WEBSITE_LOAD_CERTIFICATES"];
            
            bool validOnly = false;

                using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    certStore.Open(OpenFlags.ReadOnly);

                    X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                                X509FindType.FindByThumbprint,
                                                // Replace below with your certificate's thumbprint
                                                ThumbPrint,
                                                validOnly);
                    // Get the first cert with the thumbprint
                    X509Certificate2 cert = (X509Certificate2)certCollection.OfType<X509Certificate>().FirstOrDefault();

                    BetfairClient = new BetfairClient("UhwmsL3EqCwjEKwH");
                    BetfairClient.Login(@"/etc/client-2048.p12", "REDsky.123", "garethreid123@gmail.com", "REDsky.123", cert);
                }
#endif
        }
 }
    }