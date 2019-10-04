# oEmbed

![](http://tc.geta.no/app/rest/builds/buildType:(id:TeamFrederik_OEmbed_OEmbedBuildAndPublishNuGetPackage)/statusIcon)
[![Platform](https://img.shields.io/badge/Platform-.NET%204.6.1-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/Episerver-%2011-orange.svg?style=flat)](http://world.episerver.com/cms/) 

## Description
EPiServer integration with bit.ly. This package contains an oEmbed block (properties: URL, max width, max height) plus view. An API key from bit.ly is mandatory to use this package.

## Features
* Configuration section in web.config
* oEmbed block (URL, max width, max height) + view
* oEmbedApi class to call APi

```csharp
Install-Package Geta.oEmbed
```

After the package is installed you'll have to register a free account on embed.ly to get an API key. In your web.config you should at the bottom see &lt;oEmbedSettings apikey="your-key-here" /&gt;. Replace your-key-here with the API key from embed.ly.

Add oEmbedBlock to one of your content types:

```csharp
public virtual oEmbedBlock EmbedBlock { get; set; }
```

And in your view:

```csharp
@Html.PropertyFor(m => m.CurrentPage.EmbedBlock)
```

There's also a HTML helper method you can use:

```csharp
@Html.oEmbed(Model.CurrentPage.EmbedBlock)
```

That's it! Build and run your site. You'll now be able to embed any content from the supported 300 providers.


HTTPS support:
In order to use secured protocol you need to enable it in oEmbed settings by adding secure="true" key. Only payed embed.ly plans support SSL, otherwise you will have HTTP Status: 400 error.

You can install the latest using NuGet from [nuget.episerver.com](http://nuget.episerver.com).

You need an API key from [embed.ly](http://embed.ly) to use oEmbed. There's a free plan supported that supports upto 10 000 URLs each month (as well as paid plans with more options and support).

in Web.config:
```xml
<configSections>
    <section name="oEmbedSettings" type="Geta.oEmbed.Configuration.oEmbedSettings, Geta.oEmbed"/>
</configSections>
<oEmbedSettings apikey="your-key-here" />
```

For an introduction see: [Introducing oEmbed for EPiServer](http://www.frederikvig.com/2010/09/introducing-oembed-for-episerver/) and [New version of Geta oEmbed](http://geta.no/blogg/new-version-of-geta-oembed/).

### Cache

By default we cache the response the with a dependency on Episerver. You can disable it in web.config configuration using: DisableCache=true.

## Package maintainer
https://github.com/chipev

## Changelog

3.0.1.
  1. Added support for Episerver 11
  2. Removed old dynamic content support
  3. Removed legacy custom property
