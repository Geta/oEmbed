using System;

namespace Geta.oEmbed
{
    [Serializable]
    public class oEmbedOptions
    {
        public string Url { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public Wmode Wmode { get; set; }
    }

    public enum Wmode
    {
        window,
        transparent,
        opaque,
        direct,
        gpu
    }
}