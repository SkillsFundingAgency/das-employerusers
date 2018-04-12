namespace SFA.DAS.EmployerUsers.Support.Core.Services
{
    public interface IProvideSettings
    {
        string GetSetting(string settingKey);
        string GetNullableSetting(string settingKey);
    }
}
