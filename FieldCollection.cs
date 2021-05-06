using System.Configuration;

namespace Sayer.Config
{
    /// <summary>
    /// A collection of fields that exists within a ConfigSection. Each field represents a setting.
    /// </summary>
    public class FieldCollection : ConfigurationElementCollection
    {
        /// <inheritdoc />
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

        /// <inheritdoc />
        protected override ConfigurationElement CreateNewElement()
        {
            return new FieldElement();
        }

        /// <inheritdoc />
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FieldElement)element).Name;
        }

        /// <summary>
        /// Gets a field by its unique name within the collection
        /// </summary>
        /// <param name="name">the field name</param>
        public new FieldElement this[string name]
        {
            get => (FieldElement)BaseGet(name);

            set
            {
                if (BaseGet(name) != null)
                {
                    BaseRemove(name);
                }

                BaseAdd(value, true);
            }
        }

        /// <summary>
        /// Adds a field to the collection
        /// </summary>
        /// <param name="field">the field to add</param>
        public void Add(FieldElement field)
        {
            BaseAdd(field);
        }

        /// <summary>
        /// Removes a field from the collection
        /// </summary>
        /// <param name="field"></param>
        public void Remove(FieldElement field)
        {
            BaseRemove(field.Name);
        }

        /// <summary>
        /// Removes a field by name from the collection
        /// </summary>
        /// <param name="name">the name of the field</param>
        public void Remove(string name)
        {
            BaseRemove(name);
        }

        /// <summary>
        /// Clears the collection of all fields
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }

        /// <inheritdoc />
        protected override string ElementName => "field";
    }
}
