﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Code-gen'd

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace MyNamespace
{
    public partial class JsonContext : JsonSerializerContext
    {
        private ActiveOrUpcomingEventTypeInfo _activeOrUpcomingEvent;
        public JsonTypeInfo<ActiveOrUpcomingEvent> ActiveOrUpcomingEvent
        {
            get
            {
                if (_activeOrUpcomingEvent == null)
                {
                    _activeOrUpcomingEvent = new ActiveOrUpcomingEventTypeInfo(this);
                }

                return _activeOrUpcomingEvent.TypeInfo;
            }
        }

        private class ActiveOrUpcomingEventTypeInfo
        {
            public JsonTypeInfo<ActiveOrUpcomingEvent> TypeInfo { get; private set; }

            private JsonPropertyInfo<int> _property_Id;
            private JsonPropertyInfo<string> _property_ImageUrl;
            private JsonPropertyInfo<string> _property_Name;
            private JsonPropertyInfo<string> _property_CampaignName;
            private JsonPropertyInfo<string> _property_CampaignManagedOrganizerName;
            private JsonPropertyInfo<string> _property_Description;
            //private JsonPropertyInfo<DateTimeOffset> _property_StartDate;
            //private JsonPropertyInfo<DateTimeOffset> _property_EndDate;

            public ActiveOrUpcomingEventTypeInfo(JsonContext context)
            {
                var typeInfo = new JsonObjectInfo<ActiveOrUpcomingEvent>(CreateObjectFunc, SerializeFunc, DeserializeFunc, context.GetOptions());

                _property_Id = typeInfo.AddProperty(nameof(MyNamespace.ActiveOrUpcomingEvent.Id),
                    (obj) => { return ((ActiveOrUpcomingEvent)obj).Id; },
                    (obj, value) => { ((ActiveOrUpcomingEvent)obj).Id = value; },
                    context.Int32);

                _property_ImageUrl = typeInfo.AddProperty(nameof(MyNamespace.ActiveOrUpcomingEvent.ImageUrl),
                    (obj) => { return ((ActiveOrUpcomingEvent)obj).ImageUrl; },
                    (obj, value) => { ((ActiveOrUpcomingEvent)obj).ImageUrl = value; },
                    context.String);

                _property_Name = typeInfo.AddProperty(nameof(MyNamespace.ActiveOrUpcomingEvent.Name),
                    (obj) => { return ((ActiveOrUpcomingEvent)obj).Name; },
                    (obj, value) => { ((ActiveOrUpcomingEvent)obj).Name = value; },
                    context.String);

                _property_CampaignName = typeInfo.AddProperty(nameof(MyNamespace.ActiveOrUpcomingEvent.CampaignName),
                    (obj) => { return ((ActiveOrUpcomingEvent)obj).CampaignName; },
                    (obj, value) => { ((ActiveOrUpcomingEvent)obj).CampaignName = value; },
                    context.String);

                _property_CampaignManagedOrganizerName = typeInfo.AddProperty(nameof(MyNamespace.ActiveOrUpcomingEvent.CampaignManagedOrganizerName),
                    (obj) => { return ((ActiveOrUpcomingEvent)obj).CampaignManagedOrganizerName; },
                    (obj, value) => { ((ActiveOrUpcomingEvent)obj).CampaignManagedOrganizerName = value; },
                    context.String);

                _property_Description = typeInfo.AddProperty(nameof(MyNamespace.ActiveOrUpcomingEvent.Description),
                    (obj) => { return ((ActiveOrUpcomingEvent)obj).Description; },
                    (obj, value) => { ((ActiveOrUpcomingEvent)obj).Description = value; },
                    context.String);

                typeInfo.CompleteInitialization();
                TypeInfo = typeInfo;
            }

            private object CreateObjectFunc()
            {
                return new ActiveOrUpcomingEvent();
            }

            private void SerializeFunc(Utf8JsonWriter writer, object value, ref WriteStack writeStack, JsonSerializerOptions options)
            {
                ActiveOrUpcomingEvent obj = (ActiveOrUpcomingEvent)value;

                _property_Id.WriteValue(obj.Id, writer);
                _property_ImageUrl.WriteValue(obj.ImageUrl, writer);
                _property_Name.WriteValue(obj.Name, writer);
                _property_CampaignName.WriteValue(obj.CampaignName, writer);
                _property_CampaignManagedOrganizerName.WriteValue(obj.CampaignManagedOrganizerName, writer);
                _property_Description.WriteValue(obj.Description, writer);
            }

            private ActiveOrUpcomingEvent DeserializeFunc(ref Utf8JsonReader reader, ref ReadStack readStack, JsonSerializerOptions options)
            {
                bool ReadPropertyName(ref Utf8JsonReader reader)
                {
                    return reader.Read() && reader.TokenType == JsonTokenType.PropertyName;
                }

                ReadOnlySpan<byte> propertyName;
                ActiveOrUpcomingEvent obj = new ActiveOrUpcomingEvent();

                if (!ReadPropertyName(ref reader)) goto Done;
                propertyName = reader.ValueSpan;
                if (propertyName.SequenceEqual(_property_Id.NameAsUtf8Bytes))
                {
                    reader.Read();
                    _property_Id.ReadValueAndSetMember(ref reader, ref readStack, obj);
                    if (!ReadPropertyName(ref reader)) goto Done;
                    propertyName = reader.ValueSpan;
                }

                if (propertyName.SequenceEqual(_property_ImageUrl.NameAsUtf8Bytes))
                {
                    reader.Read();
                    _property_ImageUrl.ReadValueAndSetMember(ref reader, ref readStack, obj);
                    if (!ReadPropertyName(ref reader)) goto Done;
                    propertyName = reader.ValueSpan;
                }

                if (propertyName.SequenceEqual(_property_Name.NameAsUtf8Bytes))
                {
                    reader.Read();
                    _property_Name.ReadValueAndSetMember(ref reader, ref readStack, obj);
                    if (!ReadPropertyName(ref reader)) goto Done;
                    propertyName = reader.ValueSpan;
                }

                if (propertyName.SequenceEqual(_property_CampaignName.NameAsUtf8Bytes))
                {
                    reader.Read();
                    _property_CampaignName.ReadValueAndSetMember(ref reader, ref readStack, obj);
                    if (!ReadPropertyName(ref reader)) goto Done;
                    propertyName = reader.ValueSpan;
                }

                if (propertyName.SequenceEqual(_property_CampaignManagedOrganizerName.NameAsUtf8Bytes))
                {
                    reader.Read();
                    _property_CampaignManagedOrganizerName.ReadValueAndSetMember(ref reader, ref readStack, obj);
                    if (!ReadPropertyName(ref reader)) goto Done;
                    propertyName = reader.ValueSpan;
                }

                if (propertyName.SequenceEqual(_property_Description.NameAsUtf8Bytes))
                {
                    reader.Read();
                    _property_Description.ReadValueAndSetMember(ref reader, ref readStack, obj);
                    if (!ReadPropertyName(ref reader)) goto Done;
                    propertyName = reader.ValueSpan;
                }

                // todo:if all properties looped through, call the helper that finishes

                reader.Read();

            Done:
                if (reader.TokenType != JsonTokenType.EndObject)
                {
                    throw new JsonException("todo");
                }

                return obj;
            }
        }
    }
}
