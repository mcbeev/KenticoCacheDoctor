using System.Web;
using System.Web.Routing;

namespace KenticoCacheDoctor
{
    public class CacheDoctorHttpModule : IHttpModule
    {
        private static bool HasAppStarted = false;
        private readonly static object _syncObject = new object();

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.EndRequest += OnEndRequest;

            if (!HasAppStarted)
            {
                lock (_syncObject)
                {
                    if (!HasAppStarted)
                    {
                        // Run application StartUp code here
                        var route = new Route(Constants.RouteName, new KenticoCacheDoctorRouteHandler());
                        RouteTable.Routes.Add(Constants.RouteName, route);

                        HasAppStarted = true;
                    }
                }
            }

 
        }

        static void OnBeginRequest(object sender, System.EventArgs e)
        {
            return;
        }

        static void OnEndRequest(object sender, System.EventArgs e)
        {
            var handler = new CacheDoctorHttpHandler();

            handler.ProcessRequest(HttpContext.Current);
        }

        public void Dispose()
        {
        }
    }
}
