using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class EtsyConnectionDatum
    {
        public int Id { get; set; }
        public string EtsyApiaccessToken { get; set; }
        public string EtsyApirefreshToken { get; set; }
        public string EtsyApikey { get; set; }
        public string EtsyListingUrl { get; set; }
        public string EtsyListingInventoryUrl { get; set; }
        public string EtsyListingsUrl { get; set; }
        public string EtsyReceiptsUrl { get; set; }
        public DateTime EtsyLastReceiptPoll { get; set; }
        public long EtsyShopId { get; set; }
        public string EtsyTokenDataUrl { get; set; }
    }
}
