using System;
using System.Linq;
using EDoc2.IAppService;
using EDoc2.IAppService.Model;
using EDoc2.Sdk;
using EDoc2.ServiceProxy;
using EDoc2.ServiceProxy.Client;
using EDoc2.ServiceProxy.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectA.Services;

namespace ProjectA
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // register database
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<DocumentContext>(options => options.UseSqlite("Data Source=document.db;"));

            // register eDoc service
            SdkBaseInfo.BaseUrl = "http://doc.scivic.com.cn:8889";
            foreach (var type in AppDomain.CurrentDomain.GetAllTypes().Where(type =>
                type.IsInterface && typeof(IApplicationService).IsAssignableFrom(type)))
                serviceCollection.AddSingleton(type, builder =>
                {
                    var client = new HttpApiClient(SdkBaseInfo.BaseUrl);
                    return ProxyGenerator.CreateInterfaceProxyWithoutTarget(type, client);
                });

            // register repository service
            serviceCollection.AddSingleton<RepositoryService>();
            
            // register shepherd service
            serviceCollection.AddSingleton<ShepherdService>();
            
            // build service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // populate test data
            using var dbContext = new DocumentContext(
                serviceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            SeedData.PopulateTestData(dbContext);
            
            // run listen service
            serviceProvider.GetRequiredService<ShepherdService>().ListenEDocServer();
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}