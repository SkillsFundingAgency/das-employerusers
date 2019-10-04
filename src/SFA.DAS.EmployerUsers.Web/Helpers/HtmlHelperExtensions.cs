using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString SetZenDeskSuggestion(this HtmlHelper html, string suggestion)
        {
            return MvcHtmlString.Create($"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ search: '{suggestion}' }});</script>");
        }
    }

    public static class ZenDeskSuggestions
    {
        public static string SetUpAsAUser = "Set-up-as-a-user";
    }
}