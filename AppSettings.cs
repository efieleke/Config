using System;
using System.Configuration;

namespace Sayer.Config
{
    /// <summary>
    /// Class for retrieving settings, optionally related to a type. Settings are retrieved from a given section
    /// within the app.config file. Below is an example for a config file, which assumes the existence of class Base,
    /// as well as classes Buggy and Stable, which inherit from Base (all in the same assembly). In the example,
    /// type Buggy has log level Debug, Stable has level Error, and all other derivatives of Base have level Warn.
    /// Classes not inheriting from Base have level Info. All types have Timestamp set to true in the example.
    /// 
    /// <configuration>
    ///  <configSections>
    ///    <section name = "logSection" type="Sayer.Config.ConfigSection,Sayer.Config"/>
    ///  </configSections>
    ///  <logSection>
    ///    <field name="LogLevel" value="Info"/>
    ///    <field name="Base.LogLevel" value="Warn"/>
    ///    <field name="Base.Buggy.LogLevel" value="Debug"/>
    ///    <field name="Base.Stable.LogLevel" value="Error"/>
    ///    <field name="Timestamp" value="true"/>
    ///  </logSection>
    /// <configuration/>
    /// 
    /// The following statement, given the .config settings below, would return the value Debug:
    /// AppSettings.Get<LogLevel>("logSection", typeof(Buggy), "LogLevel");
    ///
    /// The following statement would return the value Info:
    /// AppSettings.Get<LogLevel>("logSection", "LogLevel");
    ///
    /// And the following statement would return true:
    /// AppSettings.Get<bool>("logSection", typeof(MyClass), "Timestamp")
    /// </summary>
    public class AppSettings : Settings
    {
        /// <summary>
        /// Retrieves a setting from within the given section name.
        /// 
        /// This looks for a field element with a name equal to the setting name (nothing prepended,
        /// e.g. "LogLevel").
        ///
        /// If no match is found, this will throw an exception. Otherwise the value from the matching field is returned. 
        /// </summary>
        /// <typeparam name="T">the type of setting to retrieve</typeparam>
        /// <param name="sectionName">
        /// The section name within the web.config under which the settings are stored.
        /// </param>
        /// <param name="settingName">the name of the setting</param>
        /// <returns>
        /// The setting value. This will throw an exception if the setting is not defined or if it cannot be
        /// converted to the desired type.
        /// </returns>
        public static T Get<T>(string sectionName, string settingName)
        {
            return new AppSettings(sectionName).Get<T>(settingName);
        }

        /// <summary>
        /// Retrieves a setting from within the given section name. This first looks for a field element with a name equal to
        /// the setting name prepended with the class hierarchy of the specified type argument, if not null (e.g.
        /// "BaseClass.IntermediateClass.DerivedClass.LogLevel").
        /// 
        /// If no match is found, this will look for the setting down through the type's base classes (e.g.
        /// "BaseClass.IntermediateClass.LogLevel", but stopping when leaving the assembly of the topmost type.
        ///
        /// If there is still no match (or if null was passed as the type), this will finally look for
        /// a field element with a name equal to the setting name (nothing prepended, e.g. "LogLevel").
        ///
        /// If no match is found in any of those locations, this will throw an exception. Otherwise
        /// the value from the matching field is returned. 
        /// </summary>
        /// <typeparam name="T">the type of setting to retrieve</typeparam>
        /// <param name="sectionName">
        /// The section name within the web.config under which the settings for the 'type' class hierarchy are stored.
        /// </param>
        /// <param name="type">The type that the settings are associated with. If null, the type-hierarchy is ignored.</param>
        /// <param name="settingName">the name of the setting</param>
        /// <returns>
        /// The setting value. This will throw an exception if the setting is not defined or if it cannot be
        /// converted to the desired type.
        /// </returns>
        public static T Get<T>(string sectionName, Type type, string settingName)
        {
            return new AppSettings(sectionName, type).Get<T>(settingName);
        }

        /// <summary>
        /// Sets the value for a setting within the given section name. This will store the setting in a field
        /// with the name equal to the setting name (e.g. "LogLevel").
        /// </summary>
        /// <typeparam name="T">the type of value to set</typeparam>
        /// <param name="sectionName">
        /// The section name within the web.config under which the setting is stored.
        /// </param>
        /// <param name="settingName">the name of the setting</param>
        /// <param name="value">the value to be associated with the setting</param>
        public static void Set<T>(string sectionName, string settingName, T value)
        {
            new AppSettings(sectionName).Set(settingName, value);
        }

        /// <summary>
        /// Sets the value for a setting within the given section name. This will store the setting in a
        /// type-specific location, based upon the type passed. For example, if the type is IntermediateClass,
        /// and the setting name is "LogLevel", this will store the setting in a field with the name "Base.Intermediate.LogLevel".
        /// If null is passed for the type, the field's name will just be "LogLevel" (nothing prepended).
        /// </summary>
        /// <typeparam name="T">the type of value to set</typeparam>
        /// <param name="sectionName">
        /// The section name within the web.config under which the settings for the 'type' class hierarchy are stored.
        /// </param>
        /// <param name="type">The type that the settings are associated with. If null, the type-hierarchy is ignored.</param>
        /// <param name="settingName">the name of the setting</param>
        /// <param name="value">the value to be associated with the setting</param>
        public static void Set<T>(string sectionName, Type type, string settingName, T value)
        {
            new AppSettings(sectionName, type).Set(settingName, value);
        }

        /// <summary>
        /// Constructor. Uses null for the type. Consider using this instead of the static methods to work with multiple settings.
        /// </summary>
        /// <param name="sectionName">
        /// The section name under which the settings for the 'type' class hierarchy are stored.
        /// </param>
        public AppSettings(string sectionName) : base(sectionName, null)
        {
        }

        /// <summary>
        /// Constructor. Consider using this instead of the static methods to work with multiple settings.
        /// </summary>
        /// <param name="sectionName">
        /// The section name under which the settings for the 'type' class hierarchy are stored.
        /// </param>
        /// <param name="type">
        /// The type that the settings are associated with. If null, there is no type association.
        /// </param>
        public AppSettings(string sectionName, Type type) : base(sectionName, type)
        {
        }

        /// <inheritdoc />
        protected override ConfigSection GetConfigSection()
        {
            return (ConfigSection)ConfigurationManager.GetSection(SectionName);
        }
    }
}
