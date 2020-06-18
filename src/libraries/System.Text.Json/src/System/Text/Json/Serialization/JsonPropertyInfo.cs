// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    /// <summary>
    /// todo
    /// </summary>
    [DebuggerDisplay("PropertyInfo={PropertyInfo}")]
    public abstract class JsonPropertyInfo
    {
        internal static readonly JsonPropertyInfo s_missingProperty = GetPropertyPlaceholder();

        private JsonClassInfo? _runtimeClassInfo;

        internal ClassType ClassType;

        internal JsonPropertyInfo() { }

        /// <summary>
        /// todo
        /// </summary>
        public abstract JsonConverter ConverterBase { get; set; }

        internal static JsonPropertyInfo GetPropertyPlaceholder()
        {
            JsonPropertyInfo info = new JsonPropertyInfo<object>();

            Debug.Assert(!info.IsForClassInfo);
            Debug.Assert(!info.ShouldDeserialize);
            Debug.Assert(!info.ShouldSerialize);

            info.NameAsString = string.Empty;

            return info;
        }

        // Create a property that is ignored at run-time. It uses the same type (typeof(sbyte)) to help
        // prevent issues with unsupported types and helps ensure we don't accidently (de)serialize it.
        internal static JsonPropertyInfo CreateIgnoredPropertyPlaceholder(PropertyInfo propertyInfo, JsonSerializerOptions options)
        {
            JsonPropertyInfo jsonPropertyInfo = new JsonPropertyInfo<sbyte>();
            jsonPropertyInfo.Options = options;
            jsonPropertyInfo.PropertyInfo = propertyInfo;
            jsonPropertyInfo.DeterminePropertyName();
            jsonPropertyInfo.IsIgnored = true;

            Debug.Assert(!jsonPropertyInfo.ShouldDeserialize);
            Debug.Assert(!jsonPropertyInfo.ShouldSerialize);

            return jsonPropertyInfo;
        }

        /// <summary>
        /// todo
        /// </summary>
        public Type DeclaredPropertyType { get; private set; } = null!;

        private void DeterminePropertyName()
        {
            if (PropertyInfo == null)
            {
                return;
            }

            JsonPropertyNameAttribute? nameAttribute = GetAttribute<JsonPropertyNameAttribute>(PropertyInfo);
            if (nameAttribute != null)
            {
                string name = nameAttribute.Name;
                if (name == null)
                {
                    ThrowHelper.ThrowInvalidOperationException_SerializerPropertyNameNull(ParentClassType, this);
                }

                NameAsString = name;
            }
            else if (Options.PropertyNamingPolicy != null)
            {
                string name = Options.PropertyNamingPolicy.ConvertName(PropertyInfo.Name);
                if (name == null)
                {
                    ThrowHelper.ThrowInvalidOperationException_SerializerPropertyNameNull(ParentClassType, this);
                }

                NameAsString = name;
            }
            else
            {
                NameAsString = PropertyInfo.Name;
            }

            Debug.Assert(NameAsString != null);

            NameAsUtf8Bytes = Encoding.UTF8.GetBytes(NameAsString);
            EscapedNameSection = JsonHelpers.GetEscapedPropertyNameSection(NameAsUtf8Bytes, Options.Encoder);
        }

        private void DetermineSerializationCapabilities(JsonIgnoreCondition? ignoreCondition)
        {
            if ((ClassType & (ClassType.Enumerable | ClassType.Dictionary)) == 0)
            {
                Debug.Assert(ignoreCondition != JsonIgnoreCondition.Always);

                // Three possible values for ignoreCondition:
                // null = JsonIgnore was not placed on this property, global IgnoreReadOnlyProperties wins
                // WhenNull = only ignore when null, global IgnoreReadOnlyProperties loses
                // Never = never ignore (always include), global IgnoreReadOnlyProperties loses
                bool serializeReadOnlyProperty = ignoreCondition != null || !Options.IgnoreReadOnlyProperties;

                // We serialize if there is a getter + not ignoring readonly properties.
                ShouldSerialize = HasGetter && (HasSetter || serializeReadOnlyProperty);

                // We deserialize if there is a setter.
                ShouldDeserialize = HasSetter;
            }
            else
            {
                if (HasGetter)
                {
                    Debug.Assert(ConverterBase != null);

                    ShouldSerialize = true;

                    if (HasSetter)
                    {
                        ShouldDeserialize = true;
                    }
                }
            }
        }

        private void DetermineIgnoreCondition(JsonIgnoreCondition? ignoreCondition)
        {
            if (ignoreCondition != null)
            {
                Debug.Assert(PropertyInfo != null);
                Debug.Assert(ignoreCondition != JsonIgnoreCondition.Always);

                if (ignoreCondition != JsonIgnoreCondition.Never)
                {
                    Debug.Assert(ignoreCondition == JsonIgnoreCondition.WhenWritingDefault);
                    IgnoreDefaultValuesOnWrite = true;
                }
            }
#pragma warning disable CS0618 // IgnoreNullValues is obsolete
            else if (Options.IgnoreNullValues)
            {
                Debug.Assert(Options.DefaultIgnoreCondition == JsonIgnoreCondition.Never);
                IgnoreDefaultValuesOnRead = true;
                IgnoreDefaultValuesOnWrite = true;
            }
            else if (Options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingDefault)
            {
                Debug.Assert(!Options.IgnoreNullValues);
                IgnoreDefaultValuesOnWrite = true;
            }
#pragma warning restore CS0618 // IgnoreNullValues is obsolete
        }

        internal static TAttribute? GetAttribute<TAttribute>(PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            return (TAttribute?)propertyInfo.GetCustomAttribute(typeof(TAttribute), inherit: false);
        }

        internal abstract bool GetMemberAndWriteJson(object obj, ref WriteStack state, Utf8JsonWriter writer);
        internal abstract bool GetMemberAndWriteJsonExtensionData(object obj, ref WriteStack state, Utf8JsonWriter writer);

        internal virtual void GetPolicies(JsonIgnoreCondition? ignoreCondition)
        {
            DetermineSerializationCapabilities(ignoreCondition);
            DeterminePropertyName();
            DetermineIgnoreCondition(ignoreCondition);
        }

        internal abstract object? GetValueAsObject(object obj);

        internal bool HasGetter { get; set; }
        internal bool HasSetter { get; set; }

        internal virtual void Initialize(
            Type parentClassType,
            Type declaredPropertyType,
            Type? runtimePropertyType,
            ClassType runtimeClassType,
            PropertyInfo? propertyInfo,
            JsonConverter converter,
            JsonIgnoreCondition? ignoreCondition,
            JsonSerializerOptions options)
        {
            Debug.Assert(converter != null);

            ParentClassType = parentClassType;
            DeclaredPropertyType = declaredPropertyType;
            RuntimePropertyType = runtimePropertyType;
            ClassType = runtimeClassType;
            PropertyInfo = propertyInfo;
            ConverterBase = converter;
            Options = options;
        }

        internal bool IgnoreDefaultValuesOnRead { get; private set; }
        internal bool IgnoreDefaultValuesOnWrite { get; private set; }

        /// <summary>
        /// True if the corresponding cref="JsonClassInfo.PropertyInfoForClassInfo"/> is this instance.
        /// </summary>
        internal bool IsForClassInfo { get; set; }

        // There are 3 copies of the property name:
        // 1) NameAsString. The unescaped property name.
        // 2) NameAsUtf8Bytes. The Utf8 version of NameAsString. Used during during deserialization for property lookup.
        // 3) EscapedNameSection. The escaped verson of NameAsUtf8Bytes plus the wrapping quotes and a trailing colon. Used during serialization.

        /// <summary>
        /// The unescaped name of the property.
        /// Is either the actual CLR property name,
        /// the value specified in JsonPropertyNameAttribute,
        /// or the value returned from PropertyNamingPolicy(clrPropertyName).
        /// </summary>
        public string NameAsString { get; internal set; } = null!;

        /// <summary>
        /// Utf8 version of NameAsString.
        /// </summary>
        public byte[] NameAsUtf8Bytes = null!;

        /// <summary>
        /// The escaped name passed to the writer.
        /// </summary>
        public byte[] EscapedNameSection = null!;

        // Options can be referenced here since all JsonPropertyInfos originate from a JsonClassInfo that is cached on JsonSerializerOptions.
        internal JsonSerializerOptions Options { get; set; } = null!; // initialized in Init method

        internal bool ReadJsonAndAddExtensionProperty(object obj, ref ReadStack state, ref Utf8JsonReader reader)
        {
            object propValue = GetValueAsObject(obj)!;

            if (propValue is IDictionary<string, object?> dictionaryObject)
            {
                // Handle case where extension property is System.Object-based.

                if (reader.TokenType == JsonTokenType.Null)
                {
                    // A null JSON value is treated as a null object reference.
                    dictionaryObject[state.Current.JsonPropertyNameAsString!] = null;
                }
                else
                {
                    JsonConverter<object> converter = (JsonConverter<object>)Options.GetConverter(JsonClassInfo.ObjectType);

                    if (!converter.TryRead(ref reader, typeof(JsonElement), Options, ref state, out object? value))
                    {
                        return false;
                    }

                    dictionaryObject[state.Current.JsonPropertyNameAsString!] = value;
                }
            }
            else
            {
                // Handle case where extension property is JsonElement-based.

                Debug.Assert(propValue is IDictionary<string, JsonElement>);
                IDictionary<string, JsonElement> dictionaryJsonElement = (IDictionary<string, JsonElement>)propValue;

                JsonConverter<JsonElement> converter = (JsonConverter<JsonElement>)Options.GetConverter(typeof(JsonElement));

                if (!converter.TryRead(ref reader, typeof(JsonElement), Options, ref state, out JsonElement value))
                {
                    return false;
                }

                dictionaryJsonElement[state.Current.JsonPropertyNameAsString!] = value;
            }

            return true;
        }

        internal abstract bool ReadJsonAndSetMember(object obj, ref ReadStack state, ref Utf8JsonReader reader);

        internal abstract bool ReadJsonAsObject(ref ReadStack state, ref Utf8JsonReader reader, out object? value);

        internal bool ReadJsonExtensionDataValue(ref ReadStack state, ref Utf8JsonReader reader, out object? value)
        {
            Debug.Assert(this == state.Current.JsonClassInfo.DataExtensionProperty);

            if (RuntimeClassInfo.ElementType == JsonClassInfo.ObjectType && reader.TokenType == JsonTokenType.Null)
            {
                value = null;
                return true;
            }

            JsonConverter<JsonElement> converter = (JsonConverter<JsonElement>)Options.GetConverter(typeof(JsonElement));
            if (!converter.TryRead(ref reader, typeof(JsonElement), Options, ref state, out JsonElement jsonElement))
            {
                // JsonElement is a struct that must be read in full.
                value = null;
                return false;
            }

            value = jsonElement;
            return true;
        }

        internal Type ParentClassType { get; private set; } = null!;

        internal PropertyInfo? PropertyInfo { get; private set; }

        internal JsonClassInfo RuntimeClassInfo
        {
            get
            {
                if (_runtimeClassInfo == null)
                {
                    _runtimeClassInfo = Options.GetOrAddClass(RuntimePropertyType!);
                }

                return _runtimeClassInfo;
            }
        }

        internal Type? RuntimePropertyType { get; private set; } = null;

        internal abstract void SetExtensionDictionaryAsObject(object obj, object? extensionDict);

        /// <summary>
        /// todo
        /// </summary>
        public bool ShouldSerialize { get; internal set; }

        /// <summary>
        /// todo
        /// </summary>
        public bool ShouldDeserialize { get; internal set; }

        internal bool IsIgnored { get; private set; }
    }
}
