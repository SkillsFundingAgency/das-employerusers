namespace SFA.DAS.EmployerProfiles.Domain.Configuration;

public class EnvironmentConfiguration
{
    public string EnvironmentName { get;}

    public EnvironmentConfiguration (string environmentName)
    {
        EnvironmentName = environmentName;
    }
}