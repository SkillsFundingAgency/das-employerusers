using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SFA.DAS.EmployerUsers.Support.Infrastructure.DependencyResolution
{
    [ExcludeFromCodeCoverage]
    public class LoggingPropertyFactory : ILoggingPropertyFactory
    {
        public IDictionary<string, object> GetProperties()
        {
            var properties = new Dictionary<string, object>();
            try
            {
                properties.Add("Version", GetVersion());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return properties;
        }

        private string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }
}