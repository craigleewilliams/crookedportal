using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class Gcproduct
    {
        public Gcproduct()
        {
            CrossPlatformProductListings = new HashSet<CrossPlatformProductListing>();
            GcproductImages = new HashSet<GcproductImage>();
            GcproductVariants = new HashSet<GcproductVariant>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
        public string PublishedScope { get; set; }

        public virtual ICollection<CrossPlatformProductListing> CrossPlatformProductListings { get; set; }
        public virtual ICollection<GcproductImage> GcproductImages { get; set; }
        public virtual ICollection<GcproductVariant> GcproductVariants { get; set; }
    }
}
