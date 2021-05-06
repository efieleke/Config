using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;

namespace Sayer.Config
{
    /// <summary>
    /// Helper class for retrieving settings, optionally related to a type. Settings are retrieved from a given section
    /// within the .config file.
    /// </summary>
    public abstract class Settings : IEnumerable<FieldElement>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sectionName">
        /// The section name within the .config under which the settings for the 'type' class hierarchy are stored.
        /// </param>
        /// <param name="type">The type that the settings are associated with. If null, the type-hierarchy is ignored.</param>
        protected Settings(string sectionName, Type type)
        {
            SectionName = sectionName ?? throw new NullReferenceException(nameof(sectionName));
            Type = type;
        }

        /// <summary>
        /// Tries to retrieve a setting from within the section name passed to the constructor. This first looks for a field element
        /// with a name equal to the setting name prepended with the class hierarchy of the type passed to the constructor,
        /// if not null (e.g. "BaseClass.IntermediateClass.DerivedClass.LogLevel").
        /// 
        /// If no match is found, this will look for the setting down through the type's base classes (e.g.
        /// "BaseClass.IntermediateClass.LogLevel", but stopping when leaving the assembly of the topmost type.
        ///
        /// If there is still no match (or if null was passed as the type argument to the constructor), this will finally look for
        /// a field element with a name equal to the setting name (nothing prepended, e.g. "LogLevel").
        /// 
        /// If no match is found in any of those locations, this will return false. 
        /// </summary>
        /// <typeparam name="T">the type of setting to retrieve</typeparam>
        /// <param name="settingName">the name of the setting</param>
        /// <param name="value">The value associated with the setting. Will be set to the default for T is this method returns false</param>
        /// <returns>
        /// Returns true if the setting was found, otherwise false. This will throw an exception if the setting was found but
        /// could not be converted to the desired type.
        /// </returns>
        public bool TryGet<T>(string settingName, out T value)
        {
            for (Type type = Type; type != null && type.Assembly == Type.Assembly; type = type.BaseType)
            {
                string typeName = BuildTypeName(type);

                if (TryGetSetting(typeName, settingName, out value))
                {
                    return true;
                }
            }

            // The last place to look is in 'sectionName'
            return TryGetSetting(null, settingName, out value);
        }

        /// <summary>
        /// Retrieves a setting from within the section name passed to the constructor. This first looks for a field element
        /// with a name equal to the setting name prepended with the class hierarchy of the type passed to the constructor,
        /// if not null (e.g. "BaseClass.IntermediateClass.DerivedClass.LogLevel").
        /// 
        /// If no match is found, this will look for the setting down through the type's base classes (e.g.
        /// "BaseClass.IntermediateClass.LogLevel", but stopping when leaving the assembly of the topmost type.
        ///
        /// If there is still no match (or if null was passed as the type argument to the constructor), this will finally look for
        /// a field element with a name equal to the setting name (nothing prepended, e.g. "LogLevel").
        ///
        /// If no match is found in any of those locations, this will throw an exception. Otherwise
        /// the value from the matching field is returned. 
        /// </summary>
        /// <typeparam name="T">the type of setting to retrieve</typeparam>
        /// <param name="settingName">the name of the setting</param>
        /// <returns>
        /// The setting value. This will throw an exception if the setting is not defined or if it cannot be
        /// converted to the desired type.
        /// </returns>
        public T Get<T>(string settingName)
        {
            if (TryGet(settingName, out T setting))
            {
                return setting;
            }

            throw new SettingsPropertyNotFoundException(
                $"Setting '{settingName}' not found for type {(Type == null ? "null" : Type.ToString())}. Section name is '{SectionName}'");
        }

        /// <summary>
        /// Sets the value for a setting within the section name passed to the constructor. This will store the setting in a
        /// type-specific location, based upon the type passed to the constructor. For example, if the type is IntermediateClass,
        /// and the setting name is "LogLevel", this will store the setting in a field with the name "Base.Intermediate.LogLevel".
        /// If null was passed for the type, the field's name will just be "LogLevel" (nothing prepended).
        /// </summary>
        /// <typeparam name="T">the type of value to set</typeparam>
        /// <param name="settingName">the name of the setting</param>
        /// <param name="value">the value to be associated with the setting</param>
        public void Set<T>(string settingName, T value)
        {
            string typeName = BuildTypeName(Type);
            ConfigSection settings = GetConfigSection();
            string fieldName = string.IsNullOrEmpty(typeName) ? settingName : $"{typeName}.{settingName}";

            settings.Fields[fieldName] = new FieldElement
            {
                Name = fieldName,
                Value = TypeDescriptor.GetConverter(typeof(T)).ConvertToInvariantString(value)
            };
        }

        /// <inheritdoc />
        public IEnumerator<FieldElement> GetEnumerator()
        {
            ConfigSection settings = GetConfigSection();
            if (settings == null)
            {
                throw new SettingsPropertyNotFoundException($"No config section found for section name '{SectionName}'.");
            }

            foreach (FieldElement field in settings.Fields)
            {
                yield return field;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Concrete classes must implement this by returning the ConfigSection associated with the SectionName property.
        /// </summary>
        /// <returns>The matching ConfigSection.</returns>
        protected abstract ConfigSection GetConfigSection();

        private string BuildTypeName(Type type)
        {
            var subNames = new LinkedList<string>();

            while (type != null && type.Assembly == Type.Assembly)
            {
                subNames.AddFirst(type.Name);
                type = type.BaseType;
            }

            return string.Join(".", subNames);
        }

        private bool TryGetSetting<T>(string typeName, string settingName, out T setting)
        {
            string name = string.IsNullOrEmpty(typeName) ? settingName : $"{typeName}.{settingName}";
            ConfigSection settings = GetConfigSection();
            if (settings != null)
            {
                foreach (FieldElement field in settings.Fields)
                {
                    if (string.Equals(name, field.Name))
                    {
                        setting = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(field.Value);
                        return true;
                    }
                }
            }

            setting = default;
            return false;
        }

        protected string SectionName { get; }
        private Type Type { get; }
    }
}
