using System;

namespace KenticoCacheDoctor
{
    public static class Constants
    {
        public const string RouteName = "Kentico.CacheDoctor";

        public const string RouteQueryStringVariableName = "cachekey";

        public const string KenticoResourceURLPathPrefix = "/Kentico.Resource/";

        public const string KenticoResourceABTestURLPathPrefix = "/Kentico.ABTest/";

        public const string KenticoOutputCacheKeyPageIdentifier = "outputcacheurltopagemapper";

        public const string KenticoOutputCacheKeyNodesAllIdentifierStart = "nodes|";

        public const string KenticoOutputCacheKeyNodesAllIdentifierEnd = "|all";

        public const string TableHeadingAction = "Action";

        public const string TableHeadingKey = "Key";

        public const string TableHeadingValue = "Value";

        public const string CacheKeyMarker = "_____";

        public const string LabelBust = "Bust";

        public const string LabelClearedMessage = " cleared, close and reload window opener.";

        public const string LabelAttemptingBustMessage = "Attempting to clear cache for item. ";

        public const string LabelHeaderTextCount = "Cache Item Count: ";

        public const string LabelHeaderTextSize = "Cache Size: ";
    }
}
