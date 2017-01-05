using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Mvc;
using Microsoft.Azure;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CatchAll(string path)
        {
            return RedirectToAction("NotFound", "Error", new { path });
        }

        [AuthoriseActiveUser]
        [Route("Login")]
        public ActionResult Login()
        {
            return RedirectToAction("Index");
        }


        public ActionResult CertTest()
        {
            var certificatePath = string.Format(@"{0}\bin\DasIDPCert.pfx", AppDomain.CurrentDomain.BaseDirectory);
            var codeCert = new X509Certificate2(certificatePath, "idsrv3test");

            X509Certificate2 storeCert;
            var store = new X509Store(StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                var thumbprint = CloudConfigurationManager.GetSetting("TokenCertificateThumbprint");
                var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                storeCert = certificates.Count > 0 ? certificates[0] : null;

                if (storeCert == null)
                {
                    return Content($"Failed to load cert with thumbprint {thumbprint} from store");
                }
            }
            finally
            {
                store.Close();
            }


            var details = new StringBuilder();
            details.AppendLine("Code cert");
            details.AppendLine("-------------------------");
            details.AppendLine($"FriendlyName: {codeCert.FriendlyName}");
            details.AppendLine($"PublicKey: {codeCert.GetPublicKeyString()}");
            details.AppendLine($"HasPrivateKey: {codeCert.HasPrivateKey}");
            details.AppendLine($"Thumbprint: {codeCert.Thumbprint}");
            
            details.AppendLine();

            details.AppendLine("Store cert");
            details.AppendLine("-------------------------");
            details.AppendLine($"FriendlyName: {storeCert.FriendlyName}");
            details.AppendLine($"PublicKey: {storeCert.GetPublicKeyString()}");
            details.AppendLine($"HasPrivateKey: {storeCert.HasPrivateKey}");
            details.AppendLine($"Thumbprint: {storeCert.Thumbprint}");

            return Content(details.ToString(), "text/plain");
        }
    }
}