// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Text.Json.Serialization.Converters
{
    /// <summary>
    /// Implementation of <cref>JsonObjectConverter{T}</cref> that supports the deserialization
    /// of JSON objects using parameterized constructors.
    /// </summary>
    internal sealed class ObjectCodeGenConverter<T> : ObjectDefaultConverter<T> where T : notnull
    {
        internal override bool OnTryRead(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options,
            ref ReadStack state,
            [MaybeNullWhen(false)] out T value)
        {
            //if (state.FastPath)
            {
                return base.OnTryRead(ref reader, typeToConvert, options, ref state, out value);
            }
        }

        internal override bool OnTryWrite(
            Utf8JsonWriter writer,
            T value,
            JsonSerializerOptions options,
            ref WriteStack state)
        {
            if (state.SupportContinuation || options.ReferenceHandler != null)
            {
                return base.OnTryWrite(writer, value, options, ref state);
            }

            JsonClassInfo jsonClassInfo = state.Current.JsonClassInfo;
            Debug.Assert(jsonClassInfo.Serialize != null);

            writer.WriteStartObject();
            jsonClassInfo.Serialize(writer, value, ref state, options);
            writer.WriteEndObject();

            return true;
        }
    }
}
