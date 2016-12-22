using System;
using System.Net;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class OrchestratorResponse
    {
        public OrchestratorResponse()
        {
            this.Status = HttpStatusCode.OK;
        }
        public HttpStatusCode Status { get; set; }
        public Exception Exception { get; set; }
        public string RedirectUrl { get; set; }
        public string RedirectButtonMessage { get; set; }
    }

    public class OrchestratorResponse<T> : OrchestratorResponse
    {
        public T Data { get; set; }
    }
}