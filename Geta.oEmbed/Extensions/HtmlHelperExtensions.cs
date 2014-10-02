using System.Web.Mvc;

namespace Geta.oEmbed.Extensions
{
    public static class HtmlHelperExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static MvcHtmlString oEmbed(this HtmlHelper html, oEmbedOptions oEmbedOptions)
        {
            var oEmbedResponse = oEmbedApi.Call(oEmbedOptions);

            var markup = string.Empty;
            if (oEmbedResponse != null)
                markup = oEmbedResponse.RenderMarkup();

            return new MvcHtmlString(markup);
        }
    }
}
