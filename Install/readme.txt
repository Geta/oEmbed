To use the dynamic content plugin, you need to register it in your sites episerver.config file 

<dynamicContent>
	<controls>
		...
		<add description="Displays an oEmbed object on the page" name="oEmbedPlugin" type="Geta.oEmbed.DynamicContent.oEmbedPlugin, Geta.oEmbed" />
	</controls>
</dynamicContent>

HTTPS support:
In order to use secured protocol you need to enable it in oEmbed settings by adding secure="true" key. Only payed embed.ly plans support SSL, otherwise you will have HTTP Status: 400 error.