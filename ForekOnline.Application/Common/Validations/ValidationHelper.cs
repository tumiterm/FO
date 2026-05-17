using ForekOnline.Application.Common.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ForekOnline.Application.Common.Validations
{
    public static class ValidationHelper
    {
        //
        // Summary:
        //     The maximum length for a South African ID number.
        public const int RsaIdNumberMaximumLength = 13;

        //
        // Summary:
        //     The maximum length for a South African mobile number.
        public const int RsaMobileNumberMaximumLength = 10;

        //
        // Summary:
        //     The maximum length for a South African contact number.
        public const int RsaContactNumberMaximumLength = 10;

        //
        // Summary:
        //     Formats the specified validation response. Returns the validation response contents
        //     in a formatted string which includes a breakdown of the validation response errors
        //     (if there are any).
        //
        // Parameters:
        //   validationResponse:
        //     The validation response.
        //
        // Returns:
        //     The validation response contents in a formatted string which includes a breakdown
        //     of the validation response errors (if there are any).
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The validationResponse argument is null.
        public static string Format(ValidationResponse validationResponse)
        {
            if (validationResponse == null)
            {
                throw new ArgumentNullException("validationResponse");
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Format(CultureInfo.CurrentCulture, "LocalString.FormatValidationResponseSummary", validationResponse.ErrorDescription));
            foreach (ValidationError error in validationResponse.Errors)
            {
                stringBuilder.Append(string.Format(CultureInfo.CurrentCulture, "LocalString.FormatValidationResponseDetail", error.Message, error.ParentId, error.ParentPath, error.Id));
            }

            return stringBuilder.ToString();
        }

        //
        // Summary:
        //     Displays the validation response. The validation response contents in a formatted
        //     string which includes a breakdown of the validation response errors (if there
        //     are any).
        //
        // Parameters:
        //   validationResponse:
        //     The validation response.
        //
        // Returns:
        //     The validation response contents in a formatted string which includes a breakdown
        //     of the validation response errors (if there are any).
        public static string DisplayValidationResponse(ValidationResponse validationResponse)
        {
            string text = string.Empty;
            if (!string.IsNullOrEmpty(validationResponse.Message))
            {
                text = text + validationResponse.Message + Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(validationResponse.ErrorDescription) && validationResponse.ErrorDescription != validationResponse.Message)
            {
                text = text + validationResponse.ErrorDescription + Environment.NewLine;
            }

            string text2 = ((validationResponse.Errors.Count > 1) ? "•  " : string.Empty);
            foreach (ValidationError error in validationResponse.Errors)
            {
                if (validationResponse.ErrorDescription != error.Message)
                {
                    text = text + Environment.NewLine + text2 + error.Message;
                }
            }

            return text;
        }

        //
        // Summary:
        //     Sets the validation response error.
        //
        // Parameters:
        //   errorCode:
        //     The error code.
        //
        //   errorDescription:
        //     The error description.
        //
        //   message:
        //     The message.
        //
        //   messageTitle:
        //     The message title.
        //
        //   validationResponse:
        //     The validation response.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The validationResponse argument is null.
        public static void SetValidationResponseError(int errorCode, string errorDescription, string message, string messageTitle, ValidationResponse validationResponse)
        {
            if (validationResponse == null)
            {
                throw new ArgumentNullException("validationResponse");
            }

            validationResponse.ErrorCode = errorCode;
            validationResponse.ErrorDescription = errorDescription;
            validationResponse.IsError = true;
            validationResponse.Message = message;
            validationResponse.MessageTitle = messageTitle;
        }


        //
        // Summary:
        //     Validates the property.
        //
        // Parameters:
        //   property:
        //     The property.
        //
        // Returns:
        //     Returns the validation response.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The property argument is null.
        public static ValidationResponse ValidateProperty(object property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            ValidationResponse validationResponse = new ValidationResponse();
            Collection<ValidationResult> collection = new Collection<ValidationResult>();
            Validator.TryValidateObject(property, new ValidationContext(property), collection, validateAllProperties: true);
            foreach (ValidationResult item in collection)
            {
                validationResponse.Errors.Add(new ValidationError
                {
                    Message = item.ErrorMessage
                });
            }

            if (validationResponse.Errors.Count > 0)
            {
                validationResponse.ErrorDescription = string.Format(CultureInfo.CurrentCulture, "", property.GetType().Name);
                validationResponse.IsError = true;
            }

            return validationResponse;
        }

        //
        // Summary:
        //     Validates the instance.
        //
        // Parameters:
        //   instance:
        //     The instance.
        //
        // Type parameters:
        //   TInstance:
        //     The instance type.
        //
        // Returns:
        //     Returns the validation response.
        public static ValidationResponse ValidateInstance<TInstance>(TInstance instance) where TInstance : class
        {
            return ValidateInstance(instance, isThrowError: false);
        }

        //
        // Summary:
        //     Validates the instance.
        //
        // Parameters:
        //   instance:
        //     The instance.
        //
        //   isThrowError:
        //     Whether or not to throw an error.
        //
        // Type parameters:
        //   TInstance:
        //     The instance type.
        //
        // Returns:
        //     Returns the validation response.
        public static ValidationResponse ValidateInstance<TInstance>(TInstance instance, bool isThrowError) where TInstance : class
        {
            ValidationResponse validationResponse = new ValidationResponse();
            if (instance == null)
            {
                validationResponse.ErrorDescription = string.Format(CultureInfo.CurrentCulture, "", typeof(TInstance).Name);
                validationResponse.IsError = true;
                if (isThrowError)
                {
                    throw new InvalidOperationException(validationResponse.ErrorDescription);
                }
            }

            Collection<ValidationResult> collection = new Collection<ValidationResult>();
            Validator.TryValidateObject(instance, new ValidationContext(instance), collection, validateAllProperties: true);
            foreach (ValidationResult item in collection)
            {
                validationResponse.Errors.Add(new ValidationError
                {
                    Message = item.ErrorMessage
                });
                if (isThrowError)
                {
                    throw new InvalidOperationException(item.ErrorMessage);
                }
            }

            if (validationResponse.Errors.Count > 0)
            {
                validationResponse.ErrorDescription = string.Format(CultureInfo.CurrentCulture, "", typeof(TInstance).Name);
                validationResponse.IsError = true;
            }

            return validationResponse;
        }

        //
        // Summary:
        //     Indicates whether the specified currency is valid.
        //
        // Parameters:
        //   value:
        //     The value.
        //
        //   cultureInfo:
        //     The culture information.
        //
        // Returns:
        //     Returns true if the currency is valid and false if it is invalid.
        public static bool IsValidCurrency(string value, CultureInfo cultureInfo)
        {
            decimal result;
            return decimal.TryParse(value, NumberStyles.Any, cultureInfo, out result);
        }

        //
        // Summary:
        //     Indicates whether the specified email address is valid.
        //
        // Parameters:
        //   emailAddress:
        //     The email address to validate.
        //
        // Returns:
        //     Returns true if the email address is valid and false if it is invalid.
        public static bool IsValidEmailAddress(string emailAddress)
        {
            return Regex.IsMatch(string.IsNullOrWhiteSpace(emailAddress) ? string.Empty : emailAddress, "\\b[\\w]+[\\w.-][\\w]+@[\\w]+[\\w.-]\\.[\\w]{2,4}\\b");
        }

        //
        // Summary:
        //     Indicates whether the Hanis client reference is valid.
        //
        // Parameters:
        //   hanisClientReference:
        //     The hanis client reference.
        //
        //   isHanisFromHeadOffice:
        //     if set to true [is hanis from head office].
        //
        // Returns:
        //     Returns true if the Hanis client reference is valid and false if it is invalid.
        public static bool IsValidHanisClientReference(string hanisClientReference, bool isHanisFromHeadOffice)
        {
            if (string.IsNullOrWhiteSpace(hanisClientReference))
            {
                return false;
            }

            if (isHanisFromHeadOffice)
            {
                return Regex.IsMatch(hanisClientReference, "^[Aa]([0-9]{8})$");
            }

            return Regex.IsMatch(hanisClientReference, "^[0-9]{6}$");
        }

        //
        // Summary:
        //     Indicates whether the specified hexidecimal is valid.
        //
        // Parameters:
        //   hexidecimal:
        //     The hexidecimal.
        //
        // Returns:
        //     Returns true if the hexidecimal is valid and false if it is invalid.
        public static bool IsValidHexidecimal(string hexidecimal)
        {
            return Regex.IsMatch(hexidecimal, "\\A\\b[0-9a-fA-F]+\\b\\Z");
        }

        //
        // Summary:
        //     Indicates whether the specified number is a valid modulus 10. This is also known
        //     as the Luhn algorithm.
        //
        // Parameters:
        //   number:
        //     The number.
        //
        // Returns:
        //     Returns true if the number is a valid modulus 10 and false if it is invalid.
        public static bool IsValidModulus10(string number)
        {
            if (!IsValidNumber(number))
            {
                return false;
            }

            int num = 0;
            int num2 = 0;
            for (int i = 0; i < number.Length; i++)
            {
                num2 = Convert.ToInt32(number.Substring(i, 1), CultureInfo.CurrentCulture);
                num2 = (((i + 1) % 2 == 0) ? (num2 * 2) : num2);
                num2 = ((num2 > 9) ? (num2 - 9) : num2);
                num += num2;
            }

            return num % 10 == 0;
        }

        //
        // Summary:
        //     Indicates whether the specified number is a valid modulus 11.
        //
        // Parameters:
        //   number:
        //     The number.
        //
        // Returns:
        //     Returns true if the number is a valid modulus 11 and false if it is invalid.
        public static bool IsValidModulus11(string number)
        {
            if (!IsValidNumber(number))
            {
                return false;
            }

            int num = number.Length;
            int num2 = 0;
            for (int i = 0; i < number.Length; i++)
            {
                num2 += num * int.Parse(number[i].ToString(), CultureInfo.CurrentCulture);
                num--;
            }

            return num2 - num2 / 11 * 11 == 0;
        }

        //
        // Summary:
        //     Indicates whether the specified number is valid.
        //
        // Parameters:
        //   number:
        //     The number to validate.
        //
        // Returns:
        //     Returns true if the number is valid and false if it is invalid.
        public static bool IsValidNumber(string number)
        {
            return Regex.IsMatch(string.IsNullOrWhiteSpace(number) ? string.Empty : number, "^[0-9]+$");
        }

        //
        // Summary:
        //     Indicates whether the specified percent is valid and falls within the minimum
        //     and maximum values.
        //
        // Parameters:
        //   minimumValidValue:
        //     The minimum valid value.
        //
        //   maximumValidValue:
        //     The maximum valid value.
        //
        //   percent:
        //     The percent.
        //
        // Returns:
        //     Returns true if the percent is valid and false if it is invalid.
        public static bool IsValidPercent(decimal minimumValidValue, decimal maximumValidValue, string percent)
        {
            if (IsValidCurrency(percent, CultureInfo.CurrentCulture))
            {
                decimal num = Convert.ToDecimal(percent, CultureInfo.CurrentCulture);
                if (num >= minimumValidValue && num <= maximumValidValue)
                {
                    return true;
                }
            }

            return false;
        }

        //
        // Summary:
        //     Indicates whether the specified phone number is a valid phone number.
        //
        // Parameters:
        //   phoneNumber:
        //     The phone number.
        //
        // Returns:
        //     Returns true if the phone number is valid and false if it is invalid.
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(string.IsNullOrWhiteSpace(phoneNumber) ? string.Empty : phoneNumber, "^[0-9]{10}$");
        }

        //
        // Summary:
        //     Indicates whether the specified contact number is a valid South African mobile
        //     or telephone number.
        //
        // Parameters:
        //   contactNumber:
        //     The contact number to validate.
        //
        // Returns:
        //     Returns true if the contact number is valid and false if it is invalid.
        public static bool IsValidRsaContactNumber(string contactNumber)
        {
            if (!string.IsNullOrWhiteSpace(contactNumber) && contactNumber.Trim().Length > 10)
            {
                return false;
            }

            bool flag = IsValidRsaMobileNumber(contactNumber);
            if (!flag)
            {
                flag = IsValidRsaTelephoneNumber(contactNumber);
            }

            return flag;
        }

        //
        // Summary:
        //     Indicates whether the specified identity number is a valid South African identity
        //     number.
        //
        // Parameters:
        //   identityNumber:
        //     The identity number to validate.
        //
        // Returns:
        //     Returns true if the identity number is valid and false if it is invalid.
        public static bool IsValidRsaIdentityNumber(string identityNumber)
        {
            if (identityNumber.Length == 13 && IsValidModulus10(identityNumber) && Regex.IsMatch(string.IsNullOrWhiteSpace(identityNumber) ? string.Empty : identityNumber, "(((\\d{2}((0[13578]|1[02])(0[1-9]|[12]\\d|3[01])|(0[13456789]|1[012])(0[1-9]|[12]\\d|30)|02(0[1-9]|1\\d|2[0-8])))|([02468][048]|[13579][26])0229))(( |-)(\\d{4})( |-)(\\d{3})|(\\d{7}))"))
            {
                return true;
            }

            return false;
        }

        //
        // Summary:
        //     Indicates whether the specified mobile number is a valid South African mobile
        //     number.
        //
        // Parameters:
        //   mobileNumber:
        //     The mobile number to validate.
        //
        // Returns:
        //     Returns true if the mobile number is valid and false if it is invalid.
        public static bool IsValidRsaMobileNumber(string mobileNumber)
        {
            return Regex.IsMatch(string.IsNullOrWhiteSpace(mobileNumber) ? string.Empty : mobileNumber, "(^0[87][234678]((\\d{7})|( |-)((\\d{3}))( |-)(\\d{4})|( |-)(\\d{7})))");
        }

        //
        // Summary:
        //     Indicates whether the specified postal code is a valid South African postal code.
        //
        //
        // Parameters:
        //   postalCode:
        //     The postal code to validate.
        //
        // Returns:
        //     Returns true if the postal code is valid and false if it is invalid.
        public static bool IsValidRsaPostalCode(string postalCode)
        {
            if (Regex.IsMatch(string.IsNullOrWhiteSpace(postalCode) ? string.Empty : postalCode, "^([0-9]){4}$"))
            {
                return !Regex.IsMatch(string.IsNullOrWhiteSpace(postalCode) ? string.Empty : postalCode, "^[0]{4}$");
            }

            return false;
        }

        //
        // Summary:
        //     Indicates whether the specified telephone number is a valid South African telephone
        //     number.
        //
        // Parameters:
        //   telephoneNumber:
        //     The telephone number to validate.
        //
        // Returns:
        //     Returns true if the telephone number is valid and false if it is invalid.
        public static bool IsValidRsaTelephoneNumber(string telephoneNumber)
        {
            return Regex.IsMatch(string.IsNullOrWhiteSpace(telephoneNumber) ? string.Empty : telephoneNumber, "[0](\\d{9})|([0](\\d{2})( |-)((\\d{3}))( |-)(\\d{4}))|[0](\\d{2})( |-)(\\d{7})");
        }

        //
        // Summary:
        //     Indicates whether the specified string is valid.
        //
        // Parameters:
        //   value:
        //     The string to validate.
        //
        //   validationPattern:
        //     The validation pattern.
        //
        // Returns:
        //     Returns true if the string is valid and false if it is invalid.
        public static bool IsValidString(string value, string validationPattern)
        {
            return Regex.IsMatch(string.IsNullOrWhiteSpace(value) ? string.Empty : value, validationPattern);
        }

        //
        // Summary:
        //     Indicates whether the specified number is valid and treats empty or null as valid.
        //
        //
        // Parameters:
        //   number:
        //     The number to validate.
        //
        // Returns:
        //     Returns true if the number is valid, empty or null and false otherwise.
        public static bool IsValidOrEmptyNumber(string number)
        {
            if (!Regex.IsMatch(string.IsNullOrWhiteSpace(number) ? string.Empty : number, "^[0-9]+$"))
            {
                return string.IsNullOrEmpty(number);
            }

            return true;
        }

        //
        // Summary:
        //     Indicates whether the specified number is valid and treats empty or null as valid.
        //     The number must also not be longer than the specified maximum length.
        //
        // Parameters:
        //   number:
        //     The number to validate.
        //
        //   maximumLength:
        //     The maximum allowed length of the number.
        //
        // Returns:
        //     Returns true if the number is valid, empty or null and not longer than the specified
        //     maximum length; and false otherwise.
        public static bool IsValidOrEmptyNumber(string number, int maximumLength)
        {
            if (IsValidOrEmptyNumber(number))
            {
                return (string.IsNullOrWhiteSpace(number) ? string.Empty : number).Length <= maximumLength;
            }

            return false;
        }

        //
        // Summary:
        //     Converts the string to sentence case.
        //
        // Parameters:
        //   value:
        //     The value.
        //
        // Returns:
        //     Returns the string in sentence case.
        private static string ToSentenceCase(this string value)
        {
            if (value.Length < 1)
            {
                return value;
            }

            return value.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture) + value.Substring(1, value.Length - 1).ToLower(CultureInfo.CurrentCulture);
        }
    }
}

