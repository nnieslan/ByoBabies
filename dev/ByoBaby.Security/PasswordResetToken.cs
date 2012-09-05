using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using ByoBaby.Model;
using ByoBaby.Model.Repositories;
using ByoBaby.Security;

namespace ByoBaby.Security
{
    /// <summary>
    /// Represents the password reset token for a user account.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PasswordResetTokenConverter))]
    public class PasswordResetToken
    {
        /// <summary>
        /// The number of random bytes to add to the password reset token value.
        /// </summary>
        private const int randomByteLength = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordResetToken"/> class.
        /// </summary>
        public PasswordResetToken()
        {
        }

        /// <summary>
        /// Gets or sets the person Id for the reset token.
        /// </summary>
        public long PersonId { get; set; }

        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the token expiration.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// The IP address that requested the password reset.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets an anti-forgery password reset token for the specified user.
        /// </summary>
        /// <param name="entityContext">
        /// The <see cref="ByoBabyRepository"/> context to use for database interactions.
        /// </param>
        /// <param name="personId">
        /// The Id of the person for which to get a password reset token.
        /// </param>
        /// <param name="ipAddress">
        /// The IP address from which the password reset token was requested.
        /// </param>
        /// <param name="tokenExpirationInMinutesFromNow">
        /// The time in minutes until the password reset token expires. The default is 1440 (24 hours).
        /// </param>
        /// <param name="saveToken">
        /// Whether to save the token for later validation. The default is <b>true</b>.
        /// </param>
        /// <returns>
        /// The <see cref="PasswordResetToken"/> instance containing the password reset token.
        /// </returns>
        /// <remarks>
        /// This implementation was taken in part from the WebMatrix.WebData.GeneratePasswordResetToken
        /// implementation. The WebMatrix implementation was not used in the current version of the
        /// FlowPay project to avoid destabilizing the membership management process.
        /// NOTE: The IP address is not currently used to validate the token but it is written to the 
        /// trace log.
        /// </remarks>
        public static PasswordResetToken CreateToken(ByoBabyRepository entityContext, 
            long personId,
            string ipAddress,
            int tokenExpirationInMinutesFromNow = 1440, 
            bool saveToken = true)
        {
            // Create the token value with 16 random bytes followed by the person Id.
            byte[] randomBytes;
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                randomBytes = new byte[randomByteLength];
                rng.GetBytes(randomBytes);
            }

            // Get the person Id as a byte array.
            byte[] idBytes = BitConverter.GetBytes(personId);

            // Combine the random and id byte arrays.
            byte[] tokenBytes = new byte[randomBytes.Length + idBytes.Length];
            System.Buffer.BlockCopy(randomBytes, 0, tokenBytes, 0, randomBytes.Length);
            System.Buffer.BlockCopy(idBytes, 0, tokenBytes, randomBytes.Length, idBytes.Length);

            string tokenValue = HttpServerUtility.UrlTokenEncode(tokenBytes);

            PasswordResetToken token = new PasswordResetToken
            {
                PersonId = personId,
                Value = tokenValue,
                IPAddress = ipAddress,
                Expiration = DateTime.Now.AddMinutes(tokenExpirationInMinutesFromNow)
            };

            Trace.TraceWarning("A password reset token has been created for person Id {0} requested by IP address {1}: {2}",
                    personId,
                    ipAddress,
                    tokenValue);

            if (saveToken)
            {
                Setting.SaveSetting(personId, Setting.PasswordResetTokenSettingName, token);
            }

            return token;
        }

        /// <summary>
        /// Validates whether the supplied password reset token value is valid.
        /// </summary>
        /// <param name="tokenValue">
        /// The token value to validate.
        /// </param>
        /// <param name="ipAddress">
        /// The IP Address from which the password reset token was supplied.
        /// </param>
        /// <param name="personId">
        /// The person Id for which the password reset token value is valid.
        /// </param>
        /// <returns>
        /// true if the token is valid; false otherwise.
        /// </returns>
        /// <remarks>
        /// The IP address is not currently used to validate the token but it is written to the trace log.
        /// </remarks>
        public static bool ValidateTokenValue(string tokenValue,
            string ipAddress,
            out long personId)
        {
            using (ByoBabyRepository entityContext = new ByoBabyRepository())
            {
                return ValidateTokenValue(entityContext, tokenValue, ipAddress, out personId);
            }
        }

        /// <summary>
        /// Validates whether the supplied password reset token value is valid.
        /// </summary>
        /// <param name="entityContext">
        /// The <see cref="FlowPayEntities"/> context to use for database interactions.
        /// </param>
        /// <param name="tokenValue">
        /// The token value to validate.
        /// </param>
        /// <param name="ipAddress">
        /// The IP Address from which the password reset token was supplied.
        /// </param>
        /// <param name="personId">
        /// The person Id for which the password reset token value is valid.
        /// </param>
        /// <returns>
        /// true if the token is valid; false otherwise.
        /// </returns>
        /// <remarks>
        /// The IP address is not currently used to validate the token but it is written to the trace log.
        /// </remarks>
        public static bool ValidateTokenValue(ByoBabyRepository entityContext,
            string tokenValue,
            string ipAddress,
            out long personId)
        {
            personId = 0;
            long tokenPersonId;

            try
            {
                // Attempt to extract the person Id from the token value.
                byte[] tokenBytes = HttpServerUtility.UrlTokenDecode(tokenValue);
                tokenPersonId = BitConverter.ToInt64(tokenBytes, randomByteLength);
            }
            catch (ArgumentException)
            {
                Trace.TraceWarning("An invalid password reset token value was supplied by IP address {0}: {1}",
                    ipAddress,
                    tokenValue);
                return false;
            }
            catch (FormatException)
            {
                Trace.TraceWarning("An incorrectly formatted password reset token value was supplied by IP address {0}: {1}",
                    ipAddress,
                    tokenValue);
                return false;
            }

            // Attempt to get the password reset token for the person Id.
            PasswordResetToken token = Setting.GetSettingValue<PasswordResetToken>(
                tokenPersonId,
                Setting.PasswordResetTokenSettingName);
            if (token == null)
            {
                Trace.TraceWarning("No password reset token was found for person Id {0} identified in the token value supplied by IP address {1}: {2}",
                    tokenPersonId,
                    ipAddress,
                    tokenValue);
                return false;
            }

            // Confirm that the token value is correct.
            if (token.Value != tokenValue)
            {
                Trace.TraceWarning("The password reset token provided for person Id {0} by IP address {1} is not valid: {2}",
                    tokenPersonId,
                    ipAddress,
                    tokenValue);
                return false;
            }

            // Confirm that the token has not expired.
            if (token.Expiration <= DateTime.Now)
            {
                Trace.TraceWarning("The password reset token has expired for person Id {0} provided by IP address {1}: {2}",
                    tokenPersonId,
                    ipAddress,
                    tokenValue);
                return false;
            } 

            personId = tokenPersonId;
            return true;
        }

        /// <summary>
        /// Removes the password reset token for the specified person.
        /// </summary>
        /// <param name="personId">
        /// The Id of the person for which to remove the password reset token.
        /// </param>
        public static void RemovePasswordResetToken(long personId)
        {
            Setting.RemoveSetting(personId, Setting.PasswordResetTokenSettingName);
        }
    }
}
