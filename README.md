oEmbed
======

The latest version 1.4 is compiled for .NET 4 and EPiServer 7. It will use the legacy editor to render the property in edit mode. To add oEmbed as a new property to your page types you need to use the UIHint attribute like in this example:
```csharp
[UIHint("oEmbed object")]
[BackingType(typeof(PropertyoEmbed))]
public virtual oEmbedOptions Embed { get; set; }
```

To use the dynamic content plugin, you need to register it in your sites episerver.config file 
```xml
<dynamicContent>
	<controls>
		...
		<add description="Displays an oEmbed object on the page" name="oEmbedPlugin" type="Geta.oEmbed.DynamicContent.oEmbedPlugin, Geta.oEmbed" />
	</controls>
</dynamicContent>
```
HTTPS support:
In order to use secured protocol you need to enable it in oEmbed settings by adding secure="true" key. Only payed embed.ly plans support SSL, otherwise you will have HTTP Status: 400 error.

You can install the latest using NuGet from [nuget.episerver.com] (http://nuget.episerver.com).

For an introduction see: [Introducing oEmbed for EPiServer] (http://www.frederikvig.com/2010/09/introducing-oembed-for-episerver/) and [Geta.oEmbed version 1.2 is out] (http://www.frederikvig.com/2011/11/geta-oembed-version-1-2-is-out/).