using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using EPiServer.Core;
using EPiServer.Web.PropertyControls;
using EPiServer.Web.WebControls;

namespace Geta.oEmbed.CustomProperty
{
    public class PropertyoEmbedControl : PropertyStringControl
    {
        private Property propertyUrlControl;
        private Property propertyWidthControl;
        private Property propertyHeightControl;

        public override void CreateDefaultControls()
        {
            var oEmbedControl = new oEmbedControl();

            var options = new oEmbedOptions { Url = OptionsData.Url, MaxWidth = Convert.ToInt32(OptionsData.MaxWidth), MaxHeight = Convert.ToInt32(OptionsData.MaxHeight) };

            oEmbedControl.Options = options;

            Controls.Add(oEmbedControl);
        }

        public override void CreateEditControls()
        {
            this.SetupEditControls();
        }

        protected override void SetupEditControls()
        {
            var propertyUrl = new PropertyString { Value = OptionsData.Url };
            var propertyWidth = new PropertyNumber { Value = OptionsData.MaxWidth };
            var propertyHeight = new PropertyNumber { Value = OptionsData.MaxHeight };

            propertyUrlControl = new Property(propertyUrl) { EditMode = true };
            propertyWidthControl = new Property(propertyWidth) { EditMode = true };
            propertyHeightControl = new Property(propertyHeight) { EditMode = true };

            CreateAndAddControls(propertyUrlControl, "Url to video, photo etc");
            CreateAndAddControls(propertyWidthControl, "Max width");
            CreateAndAddControls(propertyHeightControl, "Max height");
        }

        public override void ApplyEditChanges()
        {
            var data = OptionsData;

            ((IPropertyControl)propertyUrlControl.Controls[0]).ApplyChanges();
            ((IPropertyControl)propertyWidthControl.Controls[0]).ApplyChanges();
            ((IPropertyControl)propertyHeightControl.Controls[0]).ApplyChanges();

            data.Url = propertyUrlControl.InnerProperty.Value as string;
            data.MaxWidth = Convert.ToInt32(propertyWidthControl.InnerProperty.Value);
            data.MaxHeight = Convert.ToInt32(propertyHeightControl.InnerProperty.Value);

            base.SetValue(data);
        }

        private oEmbedOptions OptionsData
        {
            get
            {
                return (oEmbedOptions)this.PropertyData.Value ?? new oEmbedOptions();
            }
        }

        private void CreateAndAddControls(Control control, string label)
        {
            var div = new HtmlGenericControl("div");
            var text = new HtmlGenericControl("label") { InnerText = label + ": " };
            div.Controls.Add(text);
            div.Controls.Add(control);
            Controls.Add(div);
        }
    }
}