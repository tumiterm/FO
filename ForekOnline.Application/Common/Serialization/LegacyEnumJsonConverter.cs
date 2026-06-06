// <copyright file="LegacyEnumJsonConverter.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ForekOnline.Application.Common.Serialization
{
    /// <summary>
    /// Creates converters that tolerate the enum formats emitted by older Forek API versions.
    /// </summary>
    internal sealed class LegacyEnumJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(LegacyEnumJsonConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }

    /// <summary>
    /// Reads enum names, numeric values, display names, and separator/casing variants.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to convert.</typeparam>
    internal sealed class LegacyEnumJsonConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : struct, Enum
    {
        private static readonly IReadOnlyDictionary<string, TEnum> ValuesByNormalizedName = BuildValueMap();
        private readonly bool _hasMissingValueFallback;
        private readonly TEnum _missingValueFallback;

        public LegacyEnumJsonConverter()
        {
        }

        public LegacyEnumJsonConverter(TEnum missingValueFallback)
        {
            _hasMissingValueFallback = true;
            _missingValueFallback = missingValueFallback;
        }

        public override bool HandleNull => true;

        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return GetMissingValueOrThrow("null");
            }

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var numericValue))
            {
                return ParseNumericValue(numericValue);
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Unsupported {typeof(TEnum).Name} token {reader.TokenType}.");
            }

            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return GetMissingValueOrThrow("blank");
            }

            value = value.Trim();
            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out numericValue))
            {
                return ParseNumericValue(numericValue);
            }

            if (Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsedValue) &&
                Enum.IsDefined(typeof(TEnum), parsedValue))
            {
                return parsedValue;
            }

            if (ValuesByNormalizedName.TryGetValue(Normalize(value), out parsedValue))
            {
                return parsedValue;
            }

            throw new JsonException($"Unknown {typeof(TEnum).Name} value '{value}'.");
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());

        private static IReadOnlyDictionary<string, TEnum> BuildValueMap()
        {
            var values = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);
            foreach (var enumValue in Enum.GetValues<TEnum>())
            {
                var name = enumValue.ToString();
                values[Normalize(name)] = enumValue;

                var displayName = typeof(TEnum)
                    .GetMember(name, BindingFlags.Public | BindingFlags.Static)
                    .Single()
                    .GetCustomAttribute<DisplayAttribute>()?
                    .GetName();
                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    values[Normalize(displayName)] = enumValue;
                }
            }

            return values;
        }

        private static TEnum ParseNumericValue(long value)
        {
            var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), value);
            if (Enum.IsDefined(typeof(TEnum), enumValue))
            {
                return enumValue;
            }

            throw new JsonException($"Unknown {typeof(TEnum).Name} value '{value}'.");
        }

        private TEnum GetMissingValueOrThrow(string valueDescription)
        {
            if (_hasMissingValueFallback)
            {
                return _missingValueFallback;
            }

            throw new JsonException($"A {valueDescription} value is not valid for {typeof(TEnum).Name}.");
        }

        private static string Normalize(string value)
            => new(value.Where(char.IsLetterOrDigit).Select(char.ToUpperInvariant).ToArray());
    }
}
