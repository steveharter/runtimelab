// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    /// <summary>
    /// Represents a strongly-typed property to prevent boxing and to create a direct delegate to the getter\setter.
    /// </summary>
    /// <typeparamref name="T"/> is the <see cref="JsonConverter{T}.TypeToConvert"/> for either the property's converter,
    /// or a type's converter, if the current instance is a <see cref="JsonClassInfo.PropertyInfoForClassInfo"/>.
    public sealed class JsonPropertyInfo<T> : JsonPropertyInfo
    {
        private static readonly T s_defaultValue = default!;

        internal JsonPropertyInfo() { }

        /// <summary>
        /// todo
        /// </summary>
        public Func<object, T>? Get { get; internal set; }

        /// <summary>
        /// todo
        /// </summary>
        public Action<object, T>? Set { get; internal set; }

        /// <summary>
        /// todo
        /// </summary>
        public JsonConverter<T> Converter { get; internal set; } = null!;

        internal override void Initialize(
            Type parentClassType,
            Type declaredPropertyType,
            Type? runtimePropertyType,
            ClassType runtimeClassType,
            PropertyInfo? propertyInfo,
            JsonConverter converter,
            JsonIgnoreCondition? ignoreCondition,
            JsonSerializerOptions options)
        {
            base.Initialize(
                parentClassType,
                declaredPropertyType,
                runtimePropertyType,
                runtimeClassType,
                propertyInfo,
                converter,
                ignoreCondition,
                options);

            if (propertyInfo != null)
            {
                bool useNonPublicAccessors = GetAttribute<JsonIncludeAttribute>(propertyInfo) != null;

                MethodInfo? getMethod = propertyInfo.GetMethod;
                if (getMethod != null && (getMethod.IsPublic || useNonPublicAccessors))
                {
                    HasGetter = true;
                    Get = options.MemberAccessorStrategy.CreatePropertyGetter<T>(propertyInfo);
                }

                MethodInfo? setMethod = propertyInfo.SetMethod;
                if (setMethod != null && (setMethod.IsPublic || useNonPublicAccessors))
                {
                    HasSetter = true;
                    Set = options.MemberAccessorStrategy.CreatePropertySetter<T>(propertyInfo);
                }
            }
            else
            {
                IsForClassInfo = true;
                HasGetter = true;
                HasSetter = true;
            }

            GetPolicies(ignoreCondition);
        }

        /// <summary>
        /// todo
        /// </summary>
        public override JsonConverter ConverterBase
        {
            get
            {
                return Converter;
            }
            set
            {
                Debug.Assert(value is JsonConverter<T>);
                Converter = (JsonConverter<T>)value;
            }
        }

        internal override object? GetValueAsObject(object obj)
        {
            if (IsForClassInfo)
            {
                return obj;
            }

            Debug.Assert(HasGetter);
            return Get!(obj);
        }

        internal override bool GetMemberAndWriteJson(object obj, ref WriteStack state, Utf8JsonWriter writer)
        {
            bool success;
            T value = Get!(obj);

            if (value == null)
            {
                Debug.Assert(s_defaultValue == null && Converter.CanBeNull);

                success = true;
                if (!IgnoreDefaultValuesOnWrite)
                {
                    if (!Converter.HandleNull)
                    {
                        writer.WriteNullSection(EscapedNameSection);
                    }
                    else
                    {
                        // No object, collection, or re-entrancy converter handles null.
                        Debug.Assert(Converter.ClassType == ClassType.Value);

                        if (state.Current.PropertyState < StackFramePropertyState.Name)
                        {
                            state.Current.PropertyState = StackFramePropertyState.Name;
                            writer.WritePropertyNameSection(EscapedNameSection);
                        }

                        int originalDepth = writer.CurrentDepth;
                        Converter.Write(writer, value, Options);
                        if (originalDepth != writer.CurrentDepth)
                        {
                            ThrowHelper.ThrowJsonException_SerializationConverterWrite(Converter);
                        }
                    }
                }
            }
            else if (IgnoreDefaultValuesOnWrite && Converter._defaultComparer.Equals(s_defaultValue, value))
            {
                Debug.Assert(s_defaultValue != null && !Converter.CanBeNull);
                success = true;
            }
            else
            {
                if (state.Current.PropertyState < StackFramePropertyState.Name)
                {
                    state.Current.PropertyState = StackFramePropertyState.Name;
                    writer.WritePropertyNameSection(EscapedNameSection);
                }

                success = Converter.TryWrite(writer, value, Options, ref state);
            }

            return success;
        }

        internal override bool GetMemberAndWriteJsonExtensionData(object obj, ref WriteStack state, Utf8JsonWriter writer)
        {
            bool success;
            T value = Get!(obj);

            if (value == null)
            {
                success = true;
            }
            else
            {
                success = Converter.TryWriteDataExtensionProperty(writer, value, Options, ref state);
            }

            return success;
        }

        internal override bool ReadJsonAndSetMember(object obj, ref ReadStack state, ref Utf8JsonReader reader)
        {
            bool success;

            bool isNullToken = reader.TokenType == JsonTokenType.Null;
            if (isNullToken && !Converter.HandleNull && !state.IsContinuation)
            {
                if (!Converter.CanBeNull)
                {
                    ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Converter.TypeToConvert);
                }

                Debug.Assert(s_defaultValue == null);

                if (!IgnoreDefaultValuesOnRead)
                {
                    T value = default;
                    Set!(obj, value!);
                }

                success = true;
            }
            else if (Converter.CanUseDirectReadOrWrite)
            {
                if (!isNullToken || !IgnoreDefaultValuesOnRead || !Converter.CanBeNull)
                {
                    // Optimize for internal converters by avoiding the extra call to TryRead.
                    T fastvalue = Converter.Read(ref reader, RuntimePropertyType!, Options);
                    Set!(obj, fastvalue!);
                }

                success = true;
            }
            else
            {
                success = true;
                if (!isNullToken || !IgnoreDefaultValuesOnRead || !Converter.CanBeNull)
                {
                    success = Converter.TryRead(ref reader, RuntimePropertyType!, Options, ref state, out T value);
                    if (success)
                    {
                        Set!(obj, value!);
                    }
                }
            }

            return success;
        }

        internal override bool ReadJsonAsObject(ref ReadStack state, ref Utf8JsonReader reader, out object? value)
        {
            bool success;
            bool isNullToken = reader.TokenType == JsonTokenType.Null;
            if (isNullToken && !Converter.HandleNull && !state.IsContinuation)
            {
                if (!Converter.CanBeNull)
                {
                    ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Converter.TypeToConvert);
                }

                value = default(T)!;
                success = true;
            }
            else
            {
                // Optimize for internal converters by avoiding the extra call to TryRead.
                if (Converter.CanUseDirectReadOrWrite)
                {
                    value = Converter.Read(ref reader, RuntimePropertyType!, Options);
                    success = true;
                }
                else
                {
                    success = Converter.TryRead(ref reader, RuntimePropertyType!, Options, ref state, out T typedValue);
                    value = typedValue;
                }
            }

            return success;
        }

        internal override void SetExtensionDictionaryAsObject(object obj, object? extensionDict)
        {
            Debug.Assert(HasSetter);
            T typedValue = (T)extensionDict!;
            Set!(obj, typedValue);
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <param name="state"></param>
        /// <param name="writer"></param>
        public void WriteObject(object obj, in T value, ref WriteStack state, Utf8JsonWriter writer)
        {
            if (value == null)
            {
                Debug.Assert(s_defaultValue == null && Converter.CanBeNull);

                if (!IgnoreDefaultValuesOnWrite)
                {
                    if (!Converter.HandleNull)
                    {
                        writer.WriteNullSection(EscapedNameSection);
                    }
                    else
                    {
                        // No object, collection, or re-entrancy converter handles null.
                        Debug.Assert(Converter.ClassType == ClassType.Value);
                        writer.WritePropertyNameSection(EscapedNameSection);

                        int originalDepth = writer.CurrentDepth;
                        Converter.Write(writer, value, Options);
                        if (originalDepth != writer.CurrentDepth)
                        {
                            ThrowHelper.ThrowJsonException_SerializationConverterWrite(Converter);
                        }
                    }
                }
            }
            else if (IgnoreDefaultValuesOnWrite && Converter._defaultComparer.Equals(s_defaultValue, value))
            {
                Debug.Assert(s_defaultValue != null && !Converter.CanBeNull);
            }
            else
            {
                writer.WritePropertyNameSection(EscapedNameSection);
                if (!Converter.TryWrite(writer, value, Options, ref state))
                {
                    throw new InvalidOperationException("todo: converter returned false");
                }
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="value"></param>
        /// <param name="writer"></param>
        public void WriteValue(in T value, Utf8JsonWriter writer)
        {
            if (IgnoreDefaultValuesOnWrite && Converter._defaultComparer.Equals(s_defaultValue, value))
            {
                Debug.Assert(s_defaultValue != null && !Converter.CanBeNull);
            }
            else
            {
                // It should not be possible to get a JsonException coming from the converter or writer
                // so no need to set state.JsonPropertyNameAsString.
                writer.WritePropertyNameSection(EscapedNameSection);
                Converter.Write(writer, value, Options);
            }
        }
    }
}
