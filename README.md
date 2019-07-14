
# Mcbeev.Kentico.CacheDoctor

A library useful for working with Kentico 12 MVC and Caching. Supports .Net framework 4.6.1 through 4.7.2.

## Installation

Use NuGet to install Mcbeev.Kentico.CacheDoctor.

    Install-Package Mcbeev.Kentico.CacheDoctor

To enable, add the following to your root web.config on the MVC Live site:

```xml
<add key="KenticoCacheDoctor:Enabled" value="true"/>
```

For more information, read the [introduction blog post](https://www.mcbeev.com/KenticoCacheDoctor).
