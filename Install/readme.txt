To use the dynamic content plugin, you need to register it in your sites episerver.config file 

<dynamicContent>
	<controls>
		...
		<add description="Displays an oEmbed object on the page" name="oEmbedPlugin" type="Geta.oEmbed.DynamicContent.oEmbedPlugin, Geta.oEmbed" />
	</controls>
</dynamicContent>
