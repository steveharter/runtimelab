// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Text.Json;
using Xunit;

namespace System.Text.Json.Serialization.Tests.CodeGen
{
    public static class MyContext
    {
        public static JsonSerializerOptions Options = new JsonSerializerOptions
        {
            //todo
        };
    }

    public class SimplePoco
    {
        public static JsonClassInfo ClassInfo
        {
            get
            {
                InitializeSerializer();
                return _s_classInfo;
            }
        }

        public string MyString { get; set; }


        private static bool _s_isInitialized;
        private static JsonClassInfo _s_classInfo;

        private static JsonPropertyInfo<string> _s_Property_MyString;
        private static void InitializeSerializer()
        {
            if (_s_isInitialized)
            {
                return;
            }

            JsonSerializerOptions options = MyContext.Options;

            var classInfo = options.CreateClassInfo<SimplePoco>(SerializeFunc);

            _s_Property_MyString = classInfo.AddProperty(nameof(MyString),
                (obj) => { return ((SimplePoco)obj).MyString; },
                (obj, value) => { ((SimplePoco)obj).MyString = value; },
                options.Converters.StringConverter);

            _s_classInfo = classInfo;

            // todo: only necessary sometimes
            options.AddClassInfo(classInfo);

            _s_isInitialized = true;
        }

        private static void SerializeFunc(Utf8JsonWriter writer, object value, ref WriteStack writeStack, JsonSerializerOptions options)
        {
            SimplePoco obj = (SimplePoco)value;

            _s_Property_MyString.WriteValue(obj.MyString, writer);
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this, ClassInfo, MyContext.Options);
        }
    }

    public static class SimplePocoTests
    {
        [Fact]
        public static void Serialize()
        {
            var poco = new SimplePoco
            {
                MyString = "TEST",
            };

            string json = poco.Serialize();

            Assert.Equal(@"{""MyString"":""TEST""}", json);
        }
    }
}
