using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class EtsyListingProduct
    {
        public long Id { get; set; }
        public long ListingId { get; set; }
        public long? MctvariantId { get; set; }
        public long? GcvariantId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public string Colour { get; set; }

        public virtual EtsyListing Listing { get; set; }
    }
}
