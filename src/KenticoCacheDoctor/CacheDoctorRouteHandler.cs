using CMS.Helpers;
using System.Web;
using System.Web.Routing;
using RequestContext = System.Web.Routing.RequestContext;

namespace KenticoCacheDoctor
{
    public class KenticoCacheDoctorRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new CustomHttpHandler();  
        }
    }

    public class CustomHttpHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write(Constants.LabelAttemptingBustMessage);

            var request = context.Request;

            if (request.QueryString.Count > 0)
            {
                var cacheKey = QueryHelper.GetString(Constants.RouteQueryStringVariableName, string.Empty);
                if (!string.IsNullOrEmpty(cacheKey))
                {
                    //Hit them both just in case? Need to test this if it affect performance, for now brute force baby!
                    HttpRuntime.Cache.Remove(cacheKey);
                    CacheHelper.TouchKey(cacheKey);
                    context.Response.Write(cacheKey + Constants.LabelClearedMessage);
                }
            }
        }
    }
}
