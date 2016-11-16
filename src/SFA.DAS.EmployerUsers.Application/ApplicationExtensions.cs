using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Application
{
    internal static class ApplicationExtensions
    {
        internal static bool HasLowerCharacters(this string value)
        {
            foreach (var c in value)
            {
                if (Char.IsLower(c))
                {
                    return true;
                }
            }
            return false;
        }
        internal static bool HasUpperCharacters(this string value)
        {
            foreach (var c in value)
            {
                if (Char.IsUpper(c))
                {
                    return true;
                }
            }
            return false;
        }
        internal static bool HasNumericCharacters(this string value)
        {
            foreach (var c in value)
            {
                if (Char.IsNumber(c))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
