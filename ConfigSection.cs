using System.Configuration;

namespace Sayer.Config
{
    /// <summary>
    /// The configuration section type needed to use the Settings class. See the concrete Settings class
    /// (e.g. AppSettings) description for an example of how to use this in a .config file.
    /// </summary>
    public class ConfigSection : ConfigurationSection
    {
        /// <summary>
        /// The collection of fields within this section. Each field represents a setting.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public FieldCollection Fields => (FieldCollection)base[""];
    }
}
