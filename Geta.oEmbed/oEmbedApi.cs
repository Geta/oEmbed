using System.Configuration;
using EPiServer;
using System.Globalization;
using System.Web.Script.Serialization;

namespace Geta.oEmbed
{
    // ReSharper disable once InconsistentNaming
    public static class oEmbedApi
    {
        public static oEmbedResponse Call(oEmbedOptions options)
        {
            if (options == null
                || string.IsNullOrEmpty(options.Url))
            {
                return null;
            }

            oEmbedResponse oEmbedResponse = null;

            var jsonResponse = string.Empty;
            var endpoint = BuildUrl(options);

            try
            {
                jsonResponse = new System.Net.WebClient().DownloadString(endpoint);
            }
            catch (System.Net.WebException exception)
            {
                if (exception.Status != System.Net.WebExceptionStatus.ProtocolError)
                    throw;
            }

            if (!string.IsNullOrEmpty(jsonResponse))
                oEmbedResponse = new JavaScriptSerializer().Deserialize<oEmbedResponse>(jsonResponse);

            return oEmbedResponse;
        }

        private static string BuildUrl(oEmbedOptions options)
        {
            var endpoint = "http://api.embed.ly/1/oembed";

            if (options.MaxWidth > 0)
                endpoint = UriSupport.AddQueryString(endpoint, "maxwidth", options.MaxWidth.ToString(CultureInfo.InvariantCulture));

            if (options.MaxHeight > 0)
                endpoint = UriSupport.AddQueryString(endpoint, "maxheight", options.MaxHeight.ToString(CultureInfo.InvariantCulture));

            endpoint = UriSupport.AddQueryString(endpoint, "url", options.Url);

            if (Configuration.oEmbedSettings.Settings == null
                || string.IsNullOrEmpty(Configuration.oEmbedSettings.Settings.ApiKey))
                throw new ConfigurationErrorsException(@"Missing key for Geta.oEmbed. Add it to web.config: 
                                                        <configSections>
                                                            <section name=""oEmbedSettings"" type=""Geta.oEmbed.Configuration.oEmbedSettings, Geta.oEmbed""/>
                                                        </configSections>
                                                        <oEmbedSettings apikey=""your-key-here"" />
                                                        You can get your key at (also has free option): http://embed.ly/");

            if (Configuration.oEmbedSettings.Settings.Secure)
                endpoint = UriSupport.AddQueryString(endpoint, "secure", "true");

            endpoint = UriSupport.AddQueryString(endpoint, "key", Configuration.oEmbedSettings.Settings.ApiKey);

            return endpoint;
        }
    }
}
