using System;
using OpenQA.Selenium;

namespace SFA.DAS.EmployerUsers.EndToEndTests
{
    internal static class WebElementExtensions
    {
        internal static string GetValue(this IWebElement element)
        {
            if (element.TagName.Equals("input", StringComparison.OrdinalIgnoreCase))
            {
                return element.GetAttribute("value");
            }
            return element.Text;
        }
    }
}
