using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ByoBaby.Security
{
    /// <summary>
    /// Converts the password reset token to and from a string.
    /// </summary>
    public class PasswordResetTokenConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">
        /// An ITypeDescriptorContext that provides a format context.
        /// </param>
        /// <param name="sourceType">
        /// A Type that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) ? true : base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">
        /// An ITypeDescriptorContext that provides a format context.
        /// </param>
        /// <param name="destinationType">
        /// A Type that represents the type you want to convert to.
        /// </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string)) ? true : base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">
        /// An ITypeDescriptorContext that provides a format context. 
        /// </param>
        /// <param name="culture">
        /// The CultureInfo to use as the current culture. 
        /// </param>
        /// <param name="value">
        /// The Object to convert. 
        /// </param>
        /// <returns>
        /// An Object that represents the converted value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string stringValue = value as string;
            if (stringValue != null)
            {
                using (StringReader reader = new StringReader(stringValue))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(PasswordResetToken));
                    return serializer.Deserialize(reader);
                }
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">
        /// An ITypeDescriptorContext that provides a format context. 
        /// </param>
        /// <param name="culture">
        /// A CultureInfo. If null is passed, the current culture is assumed. 
        /// </param>
        /// <param name="value">
        /// The Object to convert. 
        /// </param>
        /// <param name="destinationType">
        /// The Type to convert the value parameter to.
        /// </param>
        /// <returns>
        /// An Object that represents the converted value.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                using (StringWriter writer = new StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(PasswordResetToken));
                    serializer.Serialize(writer, value);
                    return writer.ToString();
                }
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
