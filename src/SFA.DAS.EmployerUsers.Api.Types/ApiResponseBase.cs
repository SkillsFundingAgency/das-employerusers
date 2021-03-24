using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.EmployerUsers.Api.Types
{
    public abstract class ApiResponseBase
    {
        public bool HasError => Errors?.Count > 0;

        public IDictionary<string, string> Errors { get; set; }
    }
}
