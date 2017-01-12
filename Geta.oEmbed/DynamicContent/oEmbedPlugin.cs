using System;
using System.Web.UI;
using System.Xml.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DynamicContent;

namespace Geta.oEmbed.DynamicContent
{
    public class oEmbedPlugin : IDynamicContentBase
    {
        protected PropertyString url;
        protected PropertyNumber width;
        protected PropertyNumber height;

        public oEmbedPlugin()
        {
            url = new PropertyString { Name = "Url to video, photo etc" };
            width = new PropertyNumber { Name = "Max width" };
            height = new PropertyNumber { Name = "Max height" };
        }

        public string Render(PageBase hostPage)
        {
            throw new NotImplementedException();
        }

        public Control GetControl(PageBase hostPage)
        {
            var oEmbedControl = new oEmbedControl();

            var options = new oEmbedOptions { Url = url.ToString(), MaxWidth = Convert.ToInt32(width.Value), MaxHeight = Convert.ToInt32(height.Value) };

            oEmbedControl.Options = options;

            return oEmbedControl;
        }

        public string State
        {
            get
            {
                return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(new XElement("oembedcontent",
                    new XElement("url", url),
                    new XElement("width", width),
                    new XElement("height", height)).ToString(SaveOptions.DisableFormatting)));
            }
            set
            {
                if (value == null)
                {
                    return;
                }
 
                byte[] toDecodeByte = Convert.FromBase64String(value);
 
                var encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
 
                int charCount = utf8Decode.GetCharCount(toDecodeByte, 0, toDecodeByte.Length);
 
                var decodedChar = new char[charCount];
                utf8Decode.GetChars(toDecodeByte, 0, toDecodeByte.Length, decodedChar, 0);
 
                var oEmbedContent = XElement.Parse(new string(decodedChar));
                url.ParseToSelf((string)oEmbedContent.Element("url"));
                width.ParseToSelf((string)oEmbedContent.Element("width"));
                height.ParseToSelf((string)oEmbedContent.Element("height"));
            }
        }

        public bool RendersWithControl
        {
            get { return true; }
        }

        public PropertyDataCollection Properties
        {
            get
            {
                return new PropertyDataCollection { url, width, height };
            }
        }
    }
}