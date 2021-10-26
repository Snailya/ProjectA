using System;
using System.Linq;
using EDoc2.IAppService;
using EDoc2.IAppService.Model;
using EDoc2.Sdk;
using EDoc2.ServiceProxy;
using EDoc2.ServiceProxy.Client;
using EDoc2.ServiceProxy.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ProjectA.Test
{
    public class EDocServiceTest
    {
        private ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();

            SdkBaseInfo.BaseUrl = "http://doc.scivic.com.cn:8889";
            foreach (var type in AppDomain.CurrentDomain.GetAllTypes().Where(type =>
                type.IsInterface && typeof(IApplicationService).IsAssignableFrom(type)))
                serviceCollection.AddSingleton(type, builder =>
                {
                    var client = new HttpApiClient(SdkBaseInfo.BaseUrl);
                    return ProxyGenerator.CreateInterfaceProxyWithoutTarget(type, client);
                });

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Test]
        public void RegisterTest()
        {
            var orgAppService = _serviceProvider.GetService<IOrgAppService>();
            if (orgAppService != null) Assert.Pass();
            Assert.Fail();
        }

        [Test]
        public void IntegrationLoginTest()
        {
            var userLoginDto = new UserLoginIntegrationByUserLoginNameDto
            {
                IntegrationKey = "46aa92ec-66af-4818-b7c1-8495a9bd7f17",
                IPAddress = "192.222.222.100",
                LoginName = "6470"
            };
            var loginResult =
                _serviceProvider.GetService<IOrgAppService>()!.UserLoginIntegrationByUserLoginName(userLoginDto);
            if (loginResult.Result == 0) Assert.Pass();
            Assert.Fail();
        }
    }
}