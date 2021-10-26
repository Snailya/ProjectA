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

namespace ProjectA
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //setup our DI
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

            // build service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // populate test data
            using var dbContext = new DocumentContext(
                serviceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            if (!dbContext.Documents.Any()) SeedData.PopulateTestData(dbContext);
            dbContext.Dispose();

            // check eDoc Service
            var orgAppService = serviceProvider.GetService<IOrgAppService>();
            var token = GenerateToken(orgAppService);
            if (!string.IsNullOrEmpty(token)) Console.WriteLine($"Get token successfully: {token}");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static string GenerateToken(IOrgAppService orgAppService)
        {
            var userLoginDto = new UserLoginIntegrationByUserLoginNameDto
            {
                IntegrationKey = "46aa92ec-66af-4818-b7c1-8495a9bd7f17",
                IPAddress = "192.222.222.100",
                LoginName = "6470"
            };
            var loginResult = orgAppService.UserLoginIntegrationByUserLoginName(userLoginDto);
            return loginResult.Data;
        }
    }
}