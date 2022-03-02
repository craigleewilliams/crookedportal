using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class EtsyListing
    {
        public EtsyListing()
        {
            CrossPlatformProductListings = new HashSet<CrossPlatformProductListing>();
            EtsyListingImages = new HashSet<EtsyListingImage>();
            EtsyListingProducts = new HashSet<EtsyListingProduct>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<CrossPlatformProductListing> CrossPlatformProductListings { get; set; }
        public virtual ICollection<EtsyListingImage> EtsyListingImages { get; set; }
        public virtual ICollection<EtsyListingProduct> EtsyListingProducts { get; set; }
    }
}
