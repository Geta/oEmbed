using System.Web.UI;

namespace Geta.oEmbed
{
    // ReSharper disable once InconsistentNaming
    public class oEmbedControl : Control
    {
        public oEmbedOptions Options { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            var oEmbedResponse = oEmbedApi.Call(Options);

            if (oEmbedResponse == null)
                writer.Write("<p><a href=\"" + Options.Url + "\">" + Options.Url + "</a>. Error with embedding the source</p>");
            else
                writer.Write(oEmbedResponse.RenderMarkup());
        }
    }
}