// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MyNamespace;
using System;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace System.Text.Json.Serialization.Tests.CodeGen
{
    public class IndexViewModelTests
    {
        const string Json = "{\"Id\":1234,\"Address1\":\"The Street Name\",\"Address2\":\"20/11\",\"City\":\"The City\",\"State\":\"The State\",\"PostalCode\":\"abc-12\",\"Name\":\"Nonexisting\",\"PhoneNumber\":\"\\u002B0 11 222 333 44\",\"Country\":\"The Greatest\"}";

        //[Fact]
        //public static void Deserialize()
        //{
        //    Location obj = JsonSerializer.Deserialize(Json, JsonContext.Default.Location);

        //    Location expected = Create();
        //    // todo: Assert.Equal(
        //}

        //[Fact]
        //public static void Serialize()
        //{
        //    IndexViewModel poco = Create();
        //    string json = JsonSerializer.Serialize(poco, JsonContext.Default.IndexViewModel);
        //    Assert.Equal(Json, json);
        //}

        private static IndexViewModel Create()
        {
            return new IndexViewModel
            {
                IsNewAccount = false,
                //FeaturedCampaign = new CampaignSummaryViewModel
                //{
                //    Description = "Very nice campaing",
                //    Headline = "The Headline",
                //    Id = 234235,
                //    OrganizationName = "The Company XYZ",
                //    ImageUrl = "https://www.dotnetfoundation.org/theme/img/carousel/foundation-diagram-content.png",
                //    Title = "Promoting Open Source"
                //},
                ActiveOrUpcomingEvents = Enumerable.Repeat(
                    new MyNamespace.ActiveOrUpcomingEvent
                    {
                        Id = 10,
                        CampaignManagedOrganizerName = "Name FamiltyName",
                        CampaignName = "The very new campaing",
                        Description = "The .NET Foundation works with Microsoft and the broader industry to increase the exposure of open source projects in the .NET community and the .NET Foundation. The .NET Foundation provides access to these resources to projects and looks to promote the activities of our communities.",
                        EndDate = DateTime.UtcNow.AddYears(1),
                        Name = "Just a name",
                        ImageUrl = "https://www.dotnetfoundation.org/theme/img/carousel/foundation-diagram-content.png",
                        StartDate = DateTime.UtcNow
                    },
                    count: 20).ToList()
            };
        }
    }
}
