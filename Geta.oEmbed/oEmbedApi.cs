using System.Configuration;
using EPiServer;
using System.Globalization;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using Geta.oEmbed.Block;
using EPiServer.ServiceLocation;
using EPiServer.Framework.Cache;

namespace Geta.oEmbed
{
    // ReSharper disable once InconsistentNaming
    public static class oEmbedApi
    {
        public static oEmbedResponse Call(oEmbedBlock options)
        {
            return Call(new oEmbedOptions
            {
                Url = options.Url,
                MaxHeight = options.MaxHeight,
                MaxWidth = options.MaxWidth
            });
        }

        public static oEmbedResponse Call(oEmbedOptions options)
        {
            if (options == null || string.IsNullOrEmpty(options.Url))
            {
                return null;
            }

            oEmbedResponse oEmbedResponse = null;

            var jsonResponse = string.Empty;
            var endpoint = BuildUrl(options);

            string cacheKey = "oembed-" + endpoint;

            var _cache = ServiceLocator.Current.GetInstance<ISynchronizedObjectInstanceCache>();

            if (!Configuration.oEmbedSettings.Settings.DisableCache)
            {
                if (_cache.Get(cacheKey) != null)
                {
                    return _cache.Get<oEmbedResponse>(cacheKey, ReadStrategy.Immediate);
                }
            }

            try
            {
                jsonResponse = new System.Net.WebClient().DownloadString(endpoint);
            }
            catch (WebException exception)
            {
                if (exception.Status != System.Net.WebExceptionStatus.ProtocolError)
                {
                    throw;
                }
                else
                {
                    var response = exception.Response as HttpWebResponse;
                    if (response != null && response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        throw new ConfigurationErrorsException(@"oEmbedSettings apikey is invalid", exception);
                    }
                }
            }

            if (!string.IsNullOrEmpty(jsonResponse))
            {
                oEmbedResponse = new JavaScriptSerializer().Deserialize<oEmbedResponse>(jsonResponse);
            }

            if (!Configuration.oEmbedSettings.Settings.DisableCache)
            {
                _cache.Insert(cacheKey, oEmbedResponse, CacheEvictionPolicy.Empty);
            }

            return oEmbedResponse;
        }

        private static string BuildUrl(oEmbedOptions options)
        {
            var endpoint = "https://api.embed.ly/1/oembed";

            if (options.MaxWidth > 0)
                endpoint = UriSupport.AddQueryString(endpoint, "maxwidth", options.MaxWidth.ToString(CultureInfo.InvariantCulture));

            if (options.MaxHeight > 0)
                endpoint = UriSupport.AddQueryString(endpoint, "maxheight", options.MaxHeight.ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrEmpty(options.Url))
            {
                // First decode the URL in case the editor already inserted an encoded URL.
                endpoint = UriSupport.AddQueryString(endpoint, "url", HttpUtility.UrlEncode(HttpUtility.UrlDecode(options.Url)));
            }

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
