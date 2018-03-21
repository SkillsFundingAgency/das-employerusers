namespace SFA.DAS.EmployerUsers.Support.Core.Configuration
{
    public interface IConfigurationSettings
    {
        string EnvironmentName { get; }

        string ApplicationName { get; }
    }
}
