using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ByoBaby.Model.Repositories;

namespace ByoBaby.Model
{
    [Serializable]
    [DataContract]
    public class Setting
    {
        /// <summary>
        /// The name of the setting that contains password reset token information.
        /// </summary>
        public const string PasswordResetTokenSettingName = "PasswordResetToken";
     
        [Key]
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long? OwnerId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string TypeName { get; set; }

        [DataMember]
        public string Value { get; set; }

        #region methods

        /// <summary>
        /// Gets the value for the specified system setting.
        /// </summary>
        /// <param name="name">
        /// The name of the setting.
        /// </param>
        /// <param name="defaultValue">
        /// The default value to return if no setting is found.
        /// </param>
        public static T GetSystemSettingValue<T>(string name,
            T defaultValue = default(T))
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                return GetSystemSettingValue<T>(entityContext, name, defaultValue);
            }
        }

        /// <summary>
        /// Gets the value for the specified system setting.
        /// </summary>
        /// <param name="entityContext">
        /// The <see cref="ByoBabyRepository"/> context to use for database interactions.
        /// </param>
        /// <param name="name">
        /// The name of the setting.
        /// </param>
        /// <param name="defaultValue">
        /// The default value to return if no setting is found.
        /// </param>
        public static T GetSystemSettingValue<T>(ByoBabyRepository entityContext,
            string name,
            T defaultValue = default(T))
        {
            return GetSettingValue<T>(entityContext, null, name, defaultValue);
        }

        /// <summary>
        /// Gets the value for the specified setting.
        /// </summary>
        /// <param name="ownerId">
        /// The Id of the owner for the setting. Use <b>null</b> for system settings.
        /// </param>
        /// <param name="name">
        /// The name of the setting.
        /// </param>
        /// <param name="defaultValue">
        /// The default value to return if no setting is found.
        /// </param>
        public static T GetSettingValue<T>(
            long? ownerId,
            string name,
            T defaultValue = default(T))
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                return GetSettingValue<T>(entityContext, ownerId, name, defaultValue);
            }
        }

        /// <summary>
        /// Gets the value for the specified setting.
        /// </summary>
        /// <param name="entityContext">
        /// The <see cref="ByoBabyRepository"/> context to use for database interactions.
        /// </param>
        /// <param name="ownerId">
        /// The Id of the owner for the setting. Use <b>null</b> for system settings.
        /// </param>
        /// <param name="name">
        /// The name of the setting.
        /// </param>
        /// <param name="defaultValue">
        /// The default value to return if no setting is found.
        /// </param>
        public static T GetSettingValue<T>(ByoBabyRepository entityContext,
            long? ownerId,
            string name,
            T defaultValue = default(T))
        {
            // Attempt to get the setting.
            Setting setting = GetSetting(entityContext, ownerId, name);

            // Return the default value for the setting if the setting is not found in the database.
            if (setting == null || String.IsNullOrWhiteSpace(setting.TypeName))
            {
                return defaultValue;
            }

            // Convert and return the value as the target type.
            Type returnType = typeof(T);
            if (returnType == typeof(string))
            {
                return (T)(object)setting.Value;
            }
            else if (String.CompareOrdinal(returnType.FullName, setting.TypeName) != 0)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return (T)TypeDescriptor.GetConverter(returnType).ConvertFromInvariantString(setting.Value);
                }
                // Various exceptions can be thrown, including System.Exception.
                catch (Exception ex)
                {
                    Trace.TraceWarning("Unable to convert setting {0} (ID: {1}) with a value of {2} to a {3}. {4}", setting.Name,
                        setting.Id,
                        setting.Value,
                        returnType.FullName,
                        ex.Message);
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets the specified setting.
        /// </summary>
        /// <param name="entityContext">
        /// The <see cref="ByoBabyRepository"/> context to use for database interactions.
        /// </param>
        /// <param name="ownerId">
        /// The Id of the owner for the setting. Use <b>null</b> for system settings.
        /// </param>
        /// <param name="name">
        /// The name of the setting.
        /// </param>
        public static Setting GetSetting(ByoBabyRepository entityContext,
            long? ownerId,
            string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,
                    Strings.ArgumentMissing, "name"), "name");
            }

            // Attempt to find the current setting value.
            if (ownerId.HasValue)
            {
                return entityContext.Settings.FirstOrDefault(s => s.OwnerId == ownerId.Value
                    && s.Name == name);
            }
            else
            {
                return entityContext.Settings.FirstOrDefault(s => s.OwnerId == null
                    && s.Name == name);
            }
        }

        /// <summary>
        /// Saves the specified system setting.
        /// </summary>
        /// <param name="name">
        /// The name of the setting.
        /// </param>
        /// <param name="value">
        /// The value of the setting.
        /// </param>
        public static void SaveSystemSetting(string name, object value)
        {
            SaveSetting(null, name, value);
        }

        /// <summary>
        /// Saves the specified setting.
        /// </summary>
        /// <param name="ownerId">
        /// The Id of the owner for the setting. Use <b>null</b> for system settings.
        /// </param>
        /// <param name="name">
        /// The name of the setting.
        /// </param>
        /// <param name="value">
        /// The value of the setting.
        /// </param>
        public static void SaveSetting(long? ownerId, string name, object value)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,
                    Strings.ArgumentMissing, "name"), "name");
            }

            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                // Attempt to find the current setting value.
                Setting setting = GetSetting(entityContext, ownerId, name);

                // Create a new setting value if none is found.
                if (setting == null)
                {
                    setting = new Setting
                    {
                        OwnerId = ownerId,
                        Name = name
                    };

                    entityContext.Settings.Add(setting);
                }

                // Save the setting value based on its type.
                if (value == null)
                {
                    setting.TypeName = null;
                    setting.Value = null;
                }
                else
                {
                    Type settingType = value.GetType();

                    try
                    {
                        string stringValue = TypeDescriptor.GetConverter(settingType).ConvertToInvariantString(value);

                        setting.TypeName = settingType.FullName;
                        setting.Value = stringValue;
                    }
                    catch (NotSupportedException)
                    {
                        Trace.TraceError("Error saving setting {0}. Unable to convert the supplied value to a string.", name);
                        throw;
                    }
                }

                entityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Removes the specified setting.
        /// </summary>
        /// <param name="ownerId">
        /// The Id of the owner for the setting. Use <b>null</b> for system settings.
        /// </param>
        /// <param name="name">
        /// The name of the setting.
        /// </param>
        /// <param name="value">
        /// The value of the setting.
        /// </param>
        public static void RemoveSetting(long? ownerId, string name)
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                Setting setting = GetSetting(entityContext, ownerId, name);
                if (setting != null)
                {
                    entityContext.Settings.Remove(setting);
                    entityContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Removes the specified system setting.
        /// </summary>
        /// <param name="name">
        /// The name of the setting.
        /// </param>
        public static void RemoveSystemSetting(string name)
        {
            RemoveSetting(null, name);
        }

        #endregion
    }
}
