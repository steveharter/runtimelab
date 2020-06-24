// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Converters;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// todo
    /// </summary>
    public partial class JsonSerializerContext
    {
        private JsonTypeInfo<bool>? _boolean;
        private static JsonTypeInfo<bool>? s_boolean;
        /// <summary>
        /// todo
        /// </summary>
        public JsonTypeInfo<bool> Boolean
        {
            get
            {
                if (_boolean == null)
                {
                    if (_options.HasCustomConverters)
                    {
                        JsonConverter<bool> converter = (JsonConverter<bool>)_options.GetConverter(typeof(bool));
                        _boolean = new JsonValueInfo<bool>(converter, _options);
                    }
                    else
                    {
                        if (s_boolean == null)
                        {
                            s_boolean = new JsonValueInfo<bool>(new BooleanConverter(), _options);
                        }

                        _boolean = s_boolean;
                    }
                }

                return _boolean;
            }
        }

        private JsonTypeInfo<int>? _int32;
        private static JsonTypeInfo<int>? s_int32;
        /// <summary>
        /// todo
        /// </summary>
        public JsonTypeInfo<int> Int32
        {
            get
            {
                if (_int32 == null)
                {
                    if (_options.HasCustomConverters)
                    {
                        JsonConverter<int> converter = (JsonConverter<int>)_options.GetConverter(typeof(int));
                        _int32 = new JsonValueInfo<int>(converter, _options);
                    }
                    else
                    {
                        if (s_int32 == null)
                        {
                            s_int32 = new JsonValueInfo<int>(new Int32Converter(), _options);
                        }

                        _int32 = s_int32;
                    }
                }

                return _int32;
            }
        }

        private JsonTypeInfo<string?>? _string;
        private static JsonTypeInfo<string?>? s_string;
        /// <summary>
        /// todo
        /// </summary>
        public JsonTypeInfo<string?> String
        {
            get
            {
                if (_string == null)
                {
                    if (_options.HasCustomConverters)
                    {
                        JsonConverter<string?> converter = (JsonConverter<string?>)_options.GetConverter(typeof(string));
                        _string = new JsonValueInfo<string?>(converter, _options);
                    }
                    else
                    {
                        if (s_string == null)
                        {
                            s_string = new JsonValueInfo<string?>(new StringConverter(), _options);
                        }

                        _string = s_string;
                    }
                }

                return _string;
            }
        }
    }
}
