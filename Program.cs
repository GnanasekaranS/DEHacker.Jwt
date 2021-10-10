using DEHacker.Businesslogic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEHacker.Jwt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging((context, loggingBuilder) =>
                {
                    loggingBuilder.ClearProviders();                   
                    string instrumentationKey = context.Configuration.GetValue<string>("MySettings:APPINSIGHTS_INSTRUMENTATIONKEY");
                    if (!string.IsNullOrEmpty(instrumentationKey))
                    {
                        loggingBuilder.AddApplicationInsights(instrumentationKey);
                    }
                    else
                    {
                        loggingBuilder.AddDebug();
                        loggingBuilder.AddEventLog();
                    }

                }).ConfigureAppConfiguration((context, config) =>
                {
                    var root = config.Build();
                   // config.AddAzureKeyVault($"https://{root["KeyVault:VaultName"]}.vault.azure.net/", root["KeyVault:ClientId"], root["KeyVault:ClientSecret"]);
                });

    }
}
