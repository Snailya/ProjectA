using System;
using System.Linq;
using EDoc2.Sdk;
using EDoc2.ServiceProxy;
using EDoc2.ServiceProxy.Client;
using EDoc2.ServiceProxy.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectA.Models;
using ProjectA.Services;

namespace ProjectA.Test
{
    public class ShepherdTest
    {
        private const int DocumentEntityId = 668407;
        private const int DocVersionId = 811065;
        private ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
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

            // register shepherd service
            serviceCollection.AddSingleton<ShepherdService>();

            // build service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // populate test data
            using var dbContext = new DocumentContext(
                _serviceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            PopulateTestData(dbContext);
        }

        [Test]
        public void ListenEDocServerTest()
        {
            var shepherdService = _serviceProvider.GetRequiredService<ShepherdService>();
            shepherdService.ListenEDocServer();

            using var dbContext = new DocumentContext(
                _serviceProvider.GetRequiredService<DbContextOptions<DocumentContext>>());
            Assert.AreEqual(DocVersionId, dbContext.Documents.Find(DocumentEntityId).Versions.Last().VersionId);
        }

        private void PopulateTestData(DocumentContext context)
        {
            context.Database.EnsureCreated();
            context.Database.EnsureCreated();

            foreach (var item in context.Documents) context.Remove(item);
            context.SaveChanges();

            var document1 = new Document(DocumentEntityId, 75696);
            context.Documents.Add(document1);
            context.SaveChanges();
        }
    }
}