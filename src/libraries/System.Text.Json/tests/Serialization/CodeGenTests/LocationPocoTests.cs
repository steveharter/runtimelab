// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MyNamespace;
using System;
using System.Text.Json;
using Xunit;

namespace System.Text.Json.Serialization.Tests.CodeGen
{
    public class LocationTests
    {
        const string Json = "{\"Id\":1234,\"Address1\":\"The Street Name\",\"Address2\":\"20/11\",\"City\":\"The City\",\"State\":\"The State\",\"PostalCode\":\"abc-12\",\"Name\":\"Nonexisting\",\"PhoneNumber\":\"\\u002B0 11 222 333 44\",\"Country\":\"The Greatest\"}";

        [Fact]
        public static void Deserialize()
        {
            Location obj = JsonSerializer.Deserialize(Json, JsonContext.Default.Location);

            Location expected = Create();
            Assert.Equal(expected.Address1, obj.Address1);
            Assert.Equal(expected.Address2, obj.Address2);
            Assert.Equal(expected.City, obj.City);
            Assert.Equal(expected.State, obj.State);
            Assert.Equal(expected.PostalCode, obj.PostalCode);
            Assert.Equal(expected.Name, obj.Name);
            Assert.Equal(expected.PhoneNumber, obj.PhoneNumber);
            Assert.Equal(expected.Country, obj.Country);
        }

        [Fact]
        public static void Serialize()
        {
            Location poco = Create();
            string json = JsonSerializer.Serialize(poco, JsonContext.Default.Location);
            Assert.Equal(Json, json);
        }

        private static Location Create()
        {
            return new Location
            {
                Id = 1234,
                Address1 = "The Street Name",
                Address2 = "20/11",
                City = "The City",
                State = "The State",
                PostalCode = "abc-12",
                Name = "Nonexisting",
                PhoneNumber = "+0 11 222 333 44",
                Country = "The Greatest"
            };
        }
    }
}
