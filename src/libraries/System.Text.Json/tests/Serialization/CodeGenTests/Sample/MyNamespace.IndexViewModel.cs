// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MyNamespace
{
    public class IndexViewModel
    {
        public List<ActiveOrUpcomingEvent> ActiveOrUpcomingEvents { get; set; }
        public CampaignSummaryViewModel FeaturedCampaign { get; set; }
        public bool IsNewAccount { get; set; }
        public bool HasFeaturedCampaign => FeaturedCampaign != null;
    }
}
