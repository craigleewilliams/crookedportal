using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class EtsyListingImage
    {
        public int Id { get; set; }
        public long ImageId { get; set; }
        public long ListingId { get; set; }
        public int Rank { get; set; }
        public string Url { get; set; }
        public byte[] Image { get; set; }

        public virtual EtsyListing Listing { get; set; }
    }
}
