using EPiServer.Core;
using EPiServer.PlugIn;

namespace Geta.oEmbed.CustomProperty
{
    [PageDefinitionTypePlugIn(DisplayName = "oEmbed object")]
    public class PropertyoEmbed : GenericProperty<oEmbedOptions>
    {
        public override IPropertyControl CreatePropertyControl()
        {
            return new PropertyoEmbedControl();
        }
    }
}