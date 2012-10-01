using System.Web;

namespace Geta.oEmbed
{
    public class oEmbedResponse
    {
        public oEmbedType Type { get; set; }
        public string Version { get; set; }
        public string Title { get; set; }
        public string Author_Name { get; set; }
        public string Author_Url { get; set; }
        public string Provider_Name { get; set; }
        public string Provider_Url { get; set; }
        public string Cache_Age { get; set; }
        public string Thumbnail_Url { get; set; }
        public string Thumbnail_Width { get; set; }
        public string Thumbnail_Height { get; set; }
        public string Url { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Html { get; set; }
        

        public string RenderMarkup()
        {
            if (this.Type == oEmbedType.Photo)
            {
                return "<img src=\"" + HttpUtility.UrlPathEncode(this.Url) + "\" width=\"" + HttpUtility.HtmlEncode(this.Width) + "\" height=\"" + HttpUtility.HtmlEncode(this.Height) + "\" alt=\"" + HttpUtility.HtmlEncode(this.Title) + "\" />";
            }

            return string.IsNullOrEmpty(this.Html) ? string.Empty : this.Html;
        }
    }
}