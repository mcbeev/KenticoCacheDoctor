using CMS.Helpers;
using System.Text;
using System.Web;

namespace KenticoCacheDoctor
{
    public class CacheDoctorHttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (CacheDoctorRequestValidator.IsNotTargetRequest(context))
            {
                return;
            }

            var sb = new StringBuilder();

            var headerText = string.Format("<p>{0}{1}</p>", Constants.LabelHeaderTextCount, HttpRuntime.Cache.Count);

            var cacheSize = CacheExtensions.GetApproximateSize(HttpRuntime.Cache);

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
            var cacheItems = CacheItemsBuilder.GetAllCacheItems(HttpRuntime.Cache);

            //Run through a sorted list of all cache items
            foreach (var item in cacheItems)
            {
                var key = item.Key;
                var strongDisplay = false;

                if (key.StartsWith(Constants.CacheKeyMarker))
                {
                    key = key.Replace(Constants.CacheKeyMarker, "");
                    strongDisplay = true;
                }

                sb.AppendFormat(@"<tr><td><a href=""{4}/{0}/?{1}={2}"" target=""_blank"">{3}</a></td>",
                   Constants.RouteName,
                   Constants.RouteQueryStringVariableName,
                   key,
                   Constants.LabelBust,
                   (context.Request.ApplicationPath == "/") ? string.Empty : context.Request.ApplicationPath);

                if (item.Value is CacheItemContainer value)
                {
                    //True Kentico Cache Item, let's try to help suggest what we would Bust first
                    if (strongDisplay)
                    {
                        key = string.Format("<b>{0}</b>", key);
                    }
                    sb.AppendFormat("<td>{0}</td><td>{1}</td>", key, value.Data);
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

        public bool IsReusable => true;

    }
}
