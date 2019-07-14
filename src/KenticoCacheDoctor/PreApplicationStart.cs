using KenticoCacheDoctor;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System;
using System.Configuration;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStart), "Start")]

namespace KenticoCacheDoctor
{
    /// <summary>
    /// Dynamically load on MVC Application Start event
    /// </summary>
    public class PreApplicationStart
    {
        public static void Start()
        {
            bool enabled = Convert.ToBoolean(ConfigurationManager.AppSettings["KenticoCacheDoctor:Enabled"]);
            if (enabled)
            {
                DynamicModuleUtility.RegisterModule(typeof(CacheDoctorHttpModule));
            }
        }
    }
}
