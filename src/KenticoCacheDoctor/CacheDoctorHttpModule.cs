using System;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace KenticoCacheDoctor
{
    public class CacheDoctorHttpModule : IHttpModule
    {
        private static bool HasAppStarted = false;
        private static bool IsGlimpsePackagePresent = false;
        private static readonly object _syncObject = new object();

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
                        RouteTable.Routes.Insert(0, route);

                        var currentDomain = AppDomain.CurrentDomain;

                        if (currentDomain.GetAssemblies().Any(a => a.GetName().Name == "KenticoCacheDoctor.Glimpse"))
                        {
                            IsGlimpsePackagePresent = true;
                        }

                        HasAppStarted = true;
                    }
                }
            }


        }

        private static void OnBeginRequest(object sender, System.EventArgs e)
        {
            return;
        }

        private static void OnEndRequest(object sender, System.EventArgs e)
        {
            /*
             * We don't want to write to the Response here since Glimpse
             * will handle displaying cache information
             */
            if (IsGlimpsePackagePresent)
            {
                return;
            }

            var handler = new CacheDoctorHttpHandler();

            handler.ProcessRequest(HttpContext.Current);
        }

        public void Dispose()
        {
        }
    }
}
