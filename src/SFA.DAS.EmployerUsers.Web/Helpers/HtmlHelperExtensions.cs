using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString SetZenDeskLabels(this HtmlHelper html, params string[] labels)
        {
            var apiCallString =
                $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: [";

            var first = true;
            foreach (var label in labels)
            {
                if (!first) apiCallString += ",";
                first = false;

                apiCallString += $"'{label}'";
            }

            apiCallString += "] });</script>";

            return MvcHtmlString.Create(apiCallString);
        }
    }

    public static class ZenDeskLabels
    {
        public static string SetUpAsAUser = "reg-setup-as-a-user";
        public static string ChangeYourEmailAddress = "reg-change-your-email-address";
        public static string ChangeYourPassword = "reg-change-your-password";
    }
}