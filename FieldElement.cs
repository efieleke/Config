using System.Configuration;

namespace Sayer.Config
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    public class FieldElement : ConfigurationElement
    {
        /// <summary>
        /// The name of the setting. To make the setting specific to a type, prepend
        /// the type class hierarchy. Examples:
        /// SettingName (all types)
        /// Base.SettingName (Base type and its deriviations)
        /// Base.Intermediate.Derived.SettingName (Derived and its derivations)
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get => (string)this["name"];
            set => this["name"] = value;
        }

        /// <summary>
        /// The value associated with the setting.
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get => (string)this["value"];
            set => this["value"] = value;
        }
    }
}
