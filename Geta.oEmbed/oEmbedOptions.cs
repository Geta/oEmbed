using System;

namespace Geta.oEmbed
{
    [Serializable]
    public class oEmbedOptions
    {
        public string Url { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
    }
}