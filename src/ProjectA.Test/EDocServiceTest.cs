using System;
using System.Diagnostics;
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

        [OneTimeSetUp]
        public void Init()
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
            // act
            var orgAppService = _serviceProvider.GetService<IOrgAppService>();

            // assert
            Assert.NotNull(orgAppService);
        }

        [Test]
        public void IntegrationLoginTest()
        {
            // arrange
            var userLoginDto = new UserLoginIntegrationByUserLoginNameDto
            {
                IntegrationKey = "46aa92ec-66af-4818-b7c1-8495a9bd7f17",
                IPAddress = "192.222.222.100",
                LoginName = "6470"
            };

            // act
            var loginResult =
                _serviceProvider.GetService<IOrgAppService>()!.UserLoginIntegrationByUserLoginName(userLoginDto);

            // assert
            Assert.IsNotEmpty(loginResult.Data);
        }
    }
}