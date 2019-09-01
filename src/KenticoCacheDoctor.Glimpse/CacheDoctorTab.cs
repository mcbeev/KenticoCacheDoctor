using CMS.Helpers;
using Glimpse.AspNet.Extensibility;
using Glimpse.AspNet.Extensions;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Tab.Assist;
using System.Collections.Generic;
using System.Web;

namespace KenticoCacheDoctor.Glimpse
{
    public class CacheDoctorTab : AspNetTab, ITabLayout, ILayoutControl
    {
        public bool KeysHeadings => true;

        public override string Name => "Kentico Cache Doctor";

        private static readonly object layout;

        static CacheDoctorTab() =>
            layout = TabLayout
                .Create()
                .Cell("Statistics", TabLayout.Create().Row(r =>
                {
                    r.Cell(0).WithTitle("Name").WidthInPercent(20).AlignRight();
                    r.Cell(1).WithTitle(Constants.TableHeadingValue);
                }))
                .Cell("Cache Items", TabLayout.Create().Row(r =>
                {
                    r.Cell(0)
                        .WidthInPercent(5)
                        .AlignRight()
                        .PaddingRightInPixels(10)
                        .WithTitle(Constants.TableHeadingAction);

                    r.Cell(1)
                        .WidthInPercent(20)
                        .DisablePreview()
                        .WithTitle(Constants.TableHeadingKey);

                    r.Cell(2)
                        .WidthInPercent(75)
                        .DisablePreview()
                        .WithTitle(Constants.TableHeadingValue);
                }))
                .Build();

        public override object GetData(ITabContext context)
        {
            var httpContext = context.GetHttpContext();

            if (CacheDoctorRequestValidator.IsNotTargetRequest(httpContext.ApplicationInstance.Context))
            {
                return new Dictionary<string, object>();
            }

            var cacheItems = CacheItemsBuilder.GetAllCacheItems(HttpRuntime.Cache);

            var cacheItemsOutput = new List<object[]>
            {
                new object[] { Constants.TableHeadingAction , Constants.TableHeadingKey, Constants.TableHeadingValue }
            };

            foreach (var item in cacheItems)
            {
                var key = item.Key;
                var emphasizeKey = false;

                if (key.StartsWith(Constants.CacheKeyMarker))
                {
                    key = key.Replace(Constants.CacheKeyMarker, "");
                    emphasizeKey = true;
                }

                var data = new object[3];

                var linkTag = string.Format(
                    "<a href='{4}/{0}/?{1}={2}' target='_blank'>{3}</a>",
                    Constants.RouteName,
                    Constants.RouteQueryStringVariableName,
                    key,
                    Constants.LabelBust,
                   (httpContext.Request.ApplicationPath == "/") ? string.Empty : httpContext.Request.ApplicationPath);

                data[0] = string.Format(Formats.Raw, linkTag);

                if (key.Contains("~/kentico/bundles/"))
                {
                    data[2] = "[excluded for readability]";
                }
                else if (item.Value is CacheItemContainer containerValue)
                {
                    if (emphasizeKey)
                    {
                        key = string.Format(Formats.Strong, key);
                    }

                    if (containerValue.Data is DummyItem dummyItem)
                    {
                        key = $"{key} - {string.Format(Formats.Strong, "Dummy Key")}";

                        data[2] = new Dictionary<string, object>()
                        {
                            { "Created", containerValue.Created },
                            { "Expiration", containerValue.AbsoluteExpiration }
                        };
                    }
                    else
                    {
                        data[2] = containerValue.Data;
                    }
                }
                else
                {
                    data[2] = item.Value;
                }

                data[1] = key;

                cacheItemsOutput.Add(data);
            }

            var cacheSize = CacheExtensions.GetApproximateSize(HttpRuntime.Cache);

            return new Dictionary<string, object>()
            {
                {
                    "Statistics",
                    new object[]
                    {
                        new object[] { "Name", Constants.TableHeadingValue },
                        new object[] { Constants.LabelHeaderTextCount, HttpRuntime.Cache.Count },
                        new object[] { $"{Constants.LabelHeaderTextSize} Bytes", cacheSize, },
                        new object[] { $"{Constants.LabelHeaderTextSize} KB", cacheSize / 1024, },
                        new object[] { $"{Constants.LabelHeaderTextSize} MB", cacheSize / 1024 / 1024, }
                    }
                },
                {
                    "Cache Items", cacheItemsOutput
                }
            };
        }

        public object GetLayout() => layout;
    }
}
