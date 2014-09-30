using System.IO;
using System.Web.Mvc;
using System.Web.UI;

namespace Geta.oEmbed.Extensions
{
    public static class HtmlHelperExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static MvcHtmlString oEmbed(this HtmlHelper html, oEmbedOptions oEmbedOptions)
        {
            var stringWriter = new StringWriter();
            var oEmbedControl = new oEmbedControl
            {
                Options = oEmbedOptions
            };
            oEmbedControl.Write(new HtmlTextWriter(stringWriter));
            return new MvcHtmlString(stringWriter.ToString());
        }
    }
}
