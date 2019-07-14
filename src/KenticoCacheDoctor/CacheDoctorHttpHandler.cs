using CMS.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace KenticoCacheDoctor
{
    public class CacheDoctorHttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (IsNotTargetRequest(context))
            {
                return;
            }

            SortedDictionary<string, object> cacheitems = new SortedDictionary<string, object>();
            StringBuilder sb = new StringBuilder();

            var headerText = string.Format("<p>{0}{1}</p>", Constants.LabelHeaderTextCount, HttpRuntime.Cache.Count);

            long cacheSize = CacheExtensions.GetApproximateSize(HttpRuntime.Cache);

            headerText = string.Format("{0}<p>{1}{2} Bytes, {3} KB, {4} MB</p>",
                headerText,
                Constants.LabelHeaderTextSize,
                cacheSize,
                cacheSize / 1024,
                cacheSize / 1024 / 1024);

            sb.AppendFormat("<div style=\"width:1330px;overflow:scroll\"><h2>{0}</h2>{4}<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"3\"><thead><tr><th>{1}</th><th>{2}</th><th>{3}</th></tr></thead><tbody>", 
                Constants.RouteName,
                Constants.TableHeadingAction,
                Constants.TableHeadingKey,
                Constants.TableHeadingValue,
                headerText);
            
            //Grab a list of all items in the OutputCache, this is not sorted
            IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var key = (string)enumerator.Key;
                if (key.StartsWith(Constants.KenticoOutputCacheKeyPageIdentifier, StringComparison.OrdinalIgnoreCase) ||
                    (key.StartsWith(Constants.KenticoOutputCacheKeyNodesAllIdentifierStart, StringComparison.OrdinalIgnoreCase) && 
                    key.EndsWith(Constants.KenticoOutputCacheKeyNodesAllIdentifierEnd, StringComparison.OrdinalIgnoreCase)))
                {
                    //Sort the most important ones to the top, the ones you would want to Bust
                    key = Constants.CacheKeyMarker + key;
                }
                cacheitems.Add(key, enumerator.Value);
            }

            //Run through a sorted list of all cache items
            foreach (var item in cacheitems)
            {
                var key = item.Key;
                bool strongDisplay = false;

                if (key.StartsWith(Constants.CacheKeyMarker))
                {
                    key = key.Replace(Constants.CacheKeyMarker, "");
                    strongDisplay = true;
                }

                sb.AppendFormat(@"<tr><td><a href=""/{0}?{1}={2}"" target=""_blank"">{3}</a></td>",
                   Constants.RouteName,
                   Constants.RouteQueryStringVariableName,
                   key,
                   Constants.LabelBust);

                if (item.Value is CacheItemContainer)
                {
                    //True Kentico Cache Item, let's try to help suggest what we would Bust first
                    if(strongDisplay)
                    {
                        key = string.Format("<b>{0}</b>", key);
                    }
                    sb.AppendFormat("<td>{0}</td><td>{1}</td>", key, ((CacheItemContainer)item.Value).Data);
                }
                else
                {
                    //Not a true Kentico Cache Item, more of a generic .Net one, so let's fake it into something that is easier to print out
                    sb.AppendFormat("<td>{0}</td><td>{1}</td>", key, item.Value);
                }

                sb.Append("</tr>");
            }
            sb.Append("</tbody></table></div>");

            context.Response.Write(sb.ToString());
        }

        private bool IsNotTargetRequest(HttpContext context)
        {
            bool ignoreThisRoute = false;

            //Ensure this is not our custom route we are injecting
            if (context.Request.RawUrl.StartsWith("/"+ Constants.RouteName, StringComparison.OrdinalIgnoreCase))
            {
                ignoreThisRoute = true;
            }

            //Ensure this is an html response and not something like a JS, CSS, or Image file
            if (context.Response.ContentType == null || !context.Response.ContentType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase))
            {
                ignoreThisRoute = true;
            }

            //We don't care about Ajax Requests either, TODO why is there no .IsAjaxRequest() ?
            if (context.Request.Headers["x-requested-with"] == "XMLHttpRequest")
            {
                ignoreThisRoute = true;
            }

            //K12 SP1 bug, the JS files for ABtesting and Conversion logging have a content type of text/html..fix it Kentico!
            if(context.Request.RawUrl.StartsWith(Constants.KenticoResourceURLPathPrefix, StringComparison.OrdinalIgnoreCase) ||
                context.Request.RawUrl.StartsWith(Constants.KenticoResourceABTestURLPathPrefix, StringComparison.OrdinalIgnoreCase))
            {
                ignoreThisRoute = true;
            }

            return ignoreThisRoute;
        }

        public bool IsReusable
        {
            get { return true; }
        }

    }
}
