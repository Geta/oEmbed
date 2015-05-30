using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace Geta.oEmbed.Block
{
    // ReSharper disable once InconsistentNaming
    [ContentType(GUID = "C7AC88F4-B96B-4F33-A9AF-FCA18FAB9726", DisplayName = "oEmbed")]
    public class oEmbedBlock : BlockData
    {
        [Display(Name = "Url", Order = 100)]
        public virtual string Url { get; set; }

        [Display(Name = "Max width", Order = 140)]
        public virtual int MaxWidth { get; set; }

        [Display(Name = "Max height", Order = 120)]
        public virtual int MaxHeight { get; set; }
    }
}
