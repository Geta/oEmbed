using System.Configuration;

namespace Geta.oEmbed.Configuration
{
    public class oEmbedSettings : ConfigurationSection
    {
        private static oEmbedSettings settings = ConfigurationManager.GetSection("oEmbedSettings") as oEmbedSettings;

        public static oEmbedSettings Settings { get { return settings; } }

        [ConfigurationProperty("secure", IsRequired = false)]
        public bool Secure
        {
            get { return (bool)this["secure"]; }

            set { this["secure"] = value; }

        }

        [ConfigurationProperty("apikey", IsRequired = true)]
        public string ApiKey
        {
            get { return (string)this["apikey"]; }
            set { this["apikey"] = value; }
        }
    }
}