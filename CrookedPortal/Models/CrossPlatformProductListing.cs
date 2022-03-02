using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class CrossPlatformProductListing
    {
        public int Id { get; set; }
        public long? MctproductId { get; set; }
        public long? GcproductId { get; set; }
        public long? EtsyListingId { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual EtsyListing EtsyListing { get; set; }
        public virtual Gcproduct Gcproduct { get; set; }
        public virtual Mctproduct Mctproduct { get; set; }
    }
}
