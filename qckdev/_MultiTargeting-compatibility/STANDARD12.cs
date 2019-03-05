#if STANDARD12

namespace System.Xml.Serialization
{

    /// <summary>
    /// Indicates that a public field or property represents an XML element when the
    /// System.Xml.Serialization.XmlSerializer serializes or deserializes the object
    /// that contains it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
    internal sealed class XmlElementAttribute : Attribute
    {

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlElementAttribute
        /// class.
        /// </summary>
        public XmlElementAttribute() { }

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlElementAttribute
        /// class and specifies the name of the XML element.
        /// </summary>
        /// <param name="elementName">The XML element name of the serialized member.</param>
        public XmlElementAttribute(string elementName) { }

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlElementAttribute
        /// class and specifies a type for the member to which the System.Xml.Serialization.XmlElementAttribute
        /// is applied. This type is used by the System.Xml.Serialization.XmlSerializer when
        /// serializing or deserializing object that contains it.
        /// </summary>
        /// <param name="type">The System.Type of an object derived from the member's type.</param>
        public XmlElementAttribute(Type type) { }

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlElementAttribute
        /// and specifies the name of the XML element and a derived type for the member to
        /// which the System.Xml.Serialization.XmlElementAttribute is applied. This member
        /// type is used when the System.Xml.Serialization.XmlSerializer serializes the object
        /// that contains it.
        /// </summary>
        /// <param name="elementName">The XML element name of the serialized member.</param>
        /// <param name="type">The System.Type of an object derived from the member's type.</param>
        public XmlElementAttribute(string elementName, Type type) { }

        /// <summary>
        /// Gets or sets a value that indicates whether the System.Xml.Serialization.XmlSerializer
        /// must serialize a member that is set to null as an empty tag with the xsi:nil
        /// attribute set to true.
        /// </summary>
        /// <value>
        /// true if the System.Xml.Serialization.XmlSerializer generates the xsi:nil attribute;
        /// otherwise, false.
        /// </value>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets the name of the generated XML element.
        /// </summary>
        /// <value>
        /// Gets or sets the name of the generated XML element.
        /// </value>
        public string ElementName { get; set; }

    }

}

#else
// Ya existe.
#endif