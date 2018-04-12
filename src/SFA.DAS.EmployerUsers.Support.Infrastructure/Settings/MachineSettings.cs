using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using SFA.DAS.EmployerUsers.Support.Core.Services;

namespace SFA.DAS.EmployerUsers.Support.Infrastructure.Settings
{
    [ExcludeFromCodeCoverage]
    public sealed class MachineSettings : IProvideSettings
    {
        public string GetSetting(string settingKey)
        {
            return Environment.GetEnvironmentVariable($"DAS_{settingKey.ToUpper(CultureInfo.InvariantCulture)}", EnvironmentVariableTarget.User);
        }

        public string GetNullableSetting(string settingKey)
        {
            return GetSetting(settingKey);
        }
    }
}
