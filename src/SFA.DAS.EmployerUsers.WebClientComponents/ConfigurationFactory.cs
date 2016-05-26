namespace SFA.DAS.EmployerUsers.WebClientComponents
{
    public abstract class ConfigurationFactory
    {
        public abstract ConfigurationContext Get();


        public static ConfigurationFactory Current { get; set; }
    }
}
