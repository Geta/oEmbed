using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using EPiServer.Core;

namespace Geta.oEmbed.CustomProperty
{
    /// <summary>
    /// Custom PropertyData implementation
    /// From: http://www.ben-morris.com/episerver-creating-a-re-usable-generic-custom-property
    /// </summary>
    [Serializable]
    public abstract class GenericProperty<T> : PropertyLongString
    {
        private T _data;
        private string _originalData;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public GenericProperty()
        { }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="propertyData">The initial property data.</param>
        public GenericProperty(T propertyData)
        {
            if (propertyData == null)
            {
                throw new ArgumentNullException(string.Format("Null argument passed to {0} constructor: propertyData.",
                                                              GetType().Name));
            }

            _data = propertyData;
        }

        /// <summary>
        /// Gets the data type of the Property.
        /// </summary>
        /// <value>The data type.</value>
        public override PropertyDataType Type
        {
            get { return PropertyDataType.LongString; }
        }

        /// <summary>
        /// Gets the <see cref="System.Type"/> of the property value.
        /// </summary>
        public override Type PropertyValueType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        /// <value><c>true</c> if this instance is null; otherwise, <c>false</c>.</value>
        public override bool IsNull
        {
            get { return _data == null; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is modified.
        /// </summary>
        /// <value><c>true</c> if this instance is modified; otherwise, <c>false</c>.</value>
        public override bool IsModified
        {
            get
            {
                if (!base.IsModified)
                {
                    base.IsModified = !EqualsSerialized(_originalData);
                }

                return base.IsModified;
            }

            set
            {
                base.IsModified = value;
                if (value)
                {
                    // Reset the original value to the current one
                    _originalData = Serialize(ValueTyped);
                }
            }
        }

        /// <summary>
        /// Gets or sets a T object to be stored
        /// This is a strong typed access to the Value property.
        /// </summary>
        /// <value>The currently selected page types.</value>
        [XmlIgnore]
        public T ValueTyped
        {
            get
            {
                return _data;
            }

            set
            {
                ThrowIfReadOnly();
                if (value == null)
                {
                    Clear();
                }
                else
                {
                    _data = value;
                    Modified();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        /// <value>The value of the property.</value>
        /// <remarks>Value returns null if the property has no value defined.</remarks>
        public override object Value
        {
            get
            {
                return ValueTyped;
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                // * Check whether the object has been passed in or the serialised string
                if (value.GetType().Name == typeof(T).Name)
                {
                    ValueTyped = (T)value;
                }
                else if (value.GetType().Name == "String")
                {
                    ValueTyped = Deserialize(ToString());
                }
                else
                {
                    throw new InvalidCastException(
                        string.Format(
                            "An invalid object type \"{0}\" was passed into the property value. Expected type: \"{1}\".",
                            typeof(object).Name,
                            typeof(T).Name));
                }
            }
        }

        /// <summary>
        /// Creates an IPropertyControl that is used to display a user interface for the property.
        /// </summary>
        /// <returns>
        /// PropertyPageTypeCollectionControl that is used to displayed this property.
        /// </returns>
        /// <remarks>It is possible to change which control should be used by registering a different <see cref="T:EPiServer.Core.IPropertyControl"/> for the <see cref="T:EPiServer.Core.PropertyData"/> class in <see cref="T:EPiServer.Core.PropertyControlClassFactory"/>.</remarks>
        public abstract override IPropertyControl CreatePropertyControl();

        /// <summary>
        /// Return any internal data that will be stored to the database. "Serialize".
        /// </summary>
        /// <param name="properties">Current property collection.</param>
        /// <returns>
        /// The "serializable" value of the property.
        /// </returns>
        public override object SaveData(PropertyDataCollection properties)
        {
            return Serialize(ValueTyped);
        }

        /// <summary>
        /// Sets the internal representation from what is stored in the database. "Deserialize".
        /// </summary>
        /// <param name="value">The value.</param>
        public override void LoadData(object value)
        {
            _originalData = (string)value;
            _data = Deserialize(_originalData);
        }

        /// <summary>
        /// Creates a string that represents the selected page types.
        /// </summary>
        /// <returns>A string of page type names.</returns>
        /// <remarks>
        /// This method is used by the dynamic properties interface to display the inherited value on a page.
        /// </remarks>
        public override string ToString()
        {
            if (IsNull)
            {
                return string.Empty;
            }

            return Serialize(_data);
        }

        /// <summary>
        /// Sets the value of the property from a string representation.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        public override void ParseToSelf(string value)
        {
            Value = Deserialize(value);
        }

        /// <summary>
        /// Sets the default value for this property.
        /// </summary>
        protected override void SetDefaultValue()
        {
            ThrowIfReadOnly();
            _originalData = null;
            _data = default(T);
        }

        /// <summary>
        /// Deserializes a string version of the object and creates a list of objects from that string.
        /// </summary>
        /// <param name="data">The data to deserialize.</param>
        /// <returns>List of objects</returns>
        protected virtual T Deserialize(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(data);

                using (MemoryStream memoryStream = new MemoryStream(byteArray))
                {
                    using (StreamReader streamReader = new StreamReader(memoryStream, new UTF8Encoding(true)))
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(T));
                        T returnObj = (T)ser.Deserialize(streamReader);
                        return returnObj;
                    }
                }
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Serializes the specified list to a string.
        /// </summary>
        /// <param name="list">The list to serialize.</param>
        /// <returns>String version of list</returns>
        protected virtual string Serialize(T data)
        {
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                string xmlString = null;
                XmlSerializer xs = new XmlSerializer(typeof(T));
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8))
                {
                    xs.Serialize(xmlTextWriter, data);
                    memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                    xmlString = Encoding.UTF8.GetString(memoryStream.ToArray());
                }

                return xmlString;
            }
            finally
            {
                memoryStream.Dispose();
            }
        }

        /// <summary>
        /// Checks if the object equals the specified serialized data.
        /// </summary>
        /// <param name="serializedData">The serialized data to check.</param>
        /// <returns>True if the object equals the serialized data</returns>
        protected virtual bool EqualsSerialized(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                return IsNull;
            }

            return serializedData.Equals(Serialize(ValueTyped), StringComparison.Ordinal);
        }
    }
}