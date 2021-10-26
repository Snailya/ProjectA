using System;
using System.Linq;
using EDoc2.IAppService;
using EDoc2.IAppService.Model;
using EDoc2.Sdk;
using Microsoft.EntityFrameworkCore;
using ProjectA.Models;

namespace ProjectA.Services
{
    public class ShepherdService
    {
        private readonly DocumentContext _context;
        private readonly IFileAppService _fileAppService;
        private readonly IOrgAppService _orgAppService;
        private string _token = string.Empty;

        public ShepherdService(DocumentContext context,
            IOrgAppService orgAppService, IFileAppService fileAppService)
        {
            if (string.IsNullOrWhiteSpace(SdkBaseInfo.BaseUrl))
                throw new ArgumentNullException(nameof(SdkBaseInfo.BaseUrl), "SdkBaseInfo.BaseUrl必须先设置才能正常使用接口！");

            _context = context;
            _orgAppService = orgAppService;
            _fileAppService = fileAppService;
        }

        private void ValidateToken()
        {
            if (!string.IsNullOrEmpty(_token))
            {
                var checkTokenResult = _orgAppService.CheckUserTokenValidity(_token);
                if (checkTokenResult.Result == 0 && checkTokenResult.Data) return;
            }

            var userLoginDto = new UserLoginIntegrationByUserLoginNameDto
            {
                IntegrationKey = "46aa92ec-66af-4818-b7c1-8495a9bd7f17",
                IPAddress = "192.222.222.100",
                LoginName = "6470"
            };
            var loginResult = _orgAppService.UserLoginIntegrationByUserLoginName(userLoginDto);
            if (loginResult.Result == 0) _token = loginResult.Data;
        }

        public void ListenEDocServer()
        {
            foreach (var document in _context.Documents.Include(x => x.Snapshot))
            {
                ValidateToken();

                // get version info from EDoc Server
                var verListResult = _fileAppService.GetFileVerListByFileId(_token, document.EntityId);
                if (verListResult.Result != 0) continue; // skip if failed to get version

                foreach (var eDocFileVerInfoResult in verListResult.Data.OrderBy(x => ToDateTime(x.FileCreateTime)))
                {
                    var versionNumber = VersionNumber.FromFileCurVerNumStr(eDocFileVerInfoResult.FileCurVerNumStr);
                    if (!versionNumber.IsMajorVersion()) continue; // skip minor version

                    // update database record
                    if (document.Versions.Any(x => x.VersionNumber == versionNumber)) continue;
                    document.UpdateVersion(new DocVersion
                    {
                        VersionId = eDocFileVerInfoResult.FileCurVerId, VersionNumber = versionNumber
                    });
                }
            }

            _context.SaveChanges();
        }

        private static DateTime ToDateTime(string dateTimeString)
        {
            return DateTime.Parse(dateTimeString);
        }
    }
}