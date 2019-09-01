using System;
using System.Web;

namespace KenticoCacheDoctor
{
    public static class CacheDoctorRequestValidator
    {
        public static bool IsNotTargetRequest(HttpContext context)
        {
            //Ensure this is not our custom route we are injecting
            var appPath = (context.Request.ApplicationPath == "/") ? string.Empty : context.Request.ApplicationPath;
            appPath += "/";

            if (context.Request.RawUrl.StartsWith(appPath + Constants.RouteName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            //Ensure this is an html response and not something like a JS, CSS, or Image file
            if (context.Response.ContentType == null || !context.Response.ContentType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            //We don't care about Ajax Requests either, TODO why is there no .IsAjaxRequest() ?
            if (context.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                return true;
            }

            //K12 SP1 bug, the JS files for ABtesting and Conversion logging have a content type of text/html..fix it Kentico!
            if (context.Request.RawUrl.StartsWith(Constants.KenticoResourceURLPathPrefix, StringComparison.OrdinalIgnoreCase) ||
                context.Request.RawUrl.StartsWith(Constants.KenticoResourceABTestURLPathPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
