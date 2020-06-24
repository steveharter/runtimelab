// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Code-gen'd

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Converters;
using System.Text.Json.Serialization.Metadata;

namespace MyNamespace
{
    public partial class JsonContext : JsonSerializerContext
    {
        private class StringTypeInfo
        {
            public JsonTypeInfo<System.String> TypeInfo { get; private set; }

            public StringTypeInfo(JsonContext context)
            {
                var converter = new StringConverter();
                var typeInfo = new JsonValueInfo<System.String>(converter, context.GetOptions());
                typeInfo.CompleteInitialization();
                TypeInfo = typeInfo;
            }

            public JsonTypeInfo<Location> ClassInfo { get; private set; }
        }
    }
}
