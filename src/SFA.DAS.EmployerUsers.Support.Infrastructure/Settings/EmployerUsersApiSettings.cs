using System.Diagnostics.CodeAnalysis;
using SFA.DAS.EmployerUsers.Api.Client;

namespace SFA.DAS.EmployerUsers.Support.Infrastructure.Settings
{
    [ExcludeFromCodeCoverage]
    public class EmployerUsersApiSettings : IEmployerUsersApiConfiguration
    {
        //private readonly IProvideSettings _settings;

        //public EmployerUsersApiSettings(IProvideSettings settings)
        //{
        //    _settings = settings;
        //}

        //public string ApiBaseUrl => _settings.GetSetting("EmpUserApiBaseUrl");
        //public string ClientId => _settings.GetSetting("EmpUserApiClientId");
        //public string ClientSecret => _settings.GetSetting("EmpUserApiClientSecret");
        //public string IdentifierUri => _settings.GetSetting("EmpUserApiIdentifierUri");
        //public string Tenant => _settings.GetSetting("EmpUserApiTenant");
        //public string ClientCertificateThumbprint => _settings.GetNullableSetting("EmpUserApiCertificateThumbprint");
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
        public string ClientCertificateThumbprint { get; set; }
    }
}