using System.Configuration;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI;
using EPiServer;

namespace Geta.oEmbed
{
    public class oEmbedControl : Control
    {
        public oEmbedOptions Options { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Options == null || string.IsNullOrEmpty(Options.Url))
            {
                return;
            }

            string jsonResponse = string.Empty;
            string endpoint = this.BuildUrl();

            var webClient = new System.Net.WebClient();

            string result;

            try
            {
                jsonResponse = webClient.DownloadString(endpoint);
            }
            catch (System.Net.WebException exception)
            {
                if (exception.Status != System.Net.WebExceptionStatus.ProtocolError)
                {
                    throw;
                }
                // If it's a ProtocolError (404).
                result = "<p><a href=\"" + this.Options.Url + "\">" + this.Options.Url + "</a>. Error with embedding the source</p>";

                writer.Write(result);
            }

            if (!string.IsNullOrEmpty(jsonResponse))
            {
                var jSerialize = new JavaScriptSerializer();
                var oEmbedResponse = jSerialize.Deserialize<oEmbedResponse>(jsonResponse);
                result = oEmbedResponse.RenderMarkup();

                writer.Write(result);
            }
        }

        private string BuildUrl()
        {
            var endpoint = "http://api.embed.ly/1/oembed";

            if (this.Options.MaxWidth > 0)
            {
                endpoint = UriSupport.AddQueryString(endpoint, "maxwidth", this.Options.MaxWidth.ToString(CultureInfo.InvariantCulture));
            }
            if (this.Options.MaxHeight > 0)
            {
                endpoint = UriSupport.AddQueryString(endpoint, "maxheight", this.Options.MaxHeight.ToString(CultureInfo.InvariantCulture));
            }
            var wmode = this.Options.Wmode.ToString();
            if (!string.IsNullOrEmpty(wmode))
            {
                endpoint = UriSupport.AddQueryString(endpoint, "wmode", wmode);
            }


            endpoint = UriSupport.AddQueryString(endpoint, "url", this.Options.Url);

            if (Configuration.oEmbedSettings.Settings == null || string.IsNullOrEmpty(Configuration.oEmbedSettings.Settings.ApiKey))
            {
                throw new ConfigurationErrorsException(@"Missing key for Geta.oEmbed. Add it to web.config: 
                                                        <configSections>
                                                            <section name=""oEmbedSettings"" type=""Geta.oEmbed.Configuration.oEmbedSettings, Geta.oEmbed""/>
                                                        </configSections>
                                                        <oEmbedSettings apikey=""your-key-here"" />
                                                        You can get your key at (also has free option): http://embed.ly/");
            }

            if (Configuration.oEmbedSettings.Settings.Secure)
            {
                endpoint = UriSupport.AddQueryString(endpoint, "secure", "true");
            }

            endpoint = UriSupport.AddQueryString(endpoint, "key", Configuration.oEmbedSettings.Settings.ApiKey);

            return endpoint;
        }
    }
}