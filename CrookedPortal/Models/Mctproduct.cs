using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class Mctproduct
    {
        public Mctproduct()
        {
            CrossPlatformProductListings = new HashSet<CrossPlatformProductListing>();
            MctproductImages = new HashSet<MctproductImage>();
            MctproductVariants = new HashSet<MctproductVariant>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
        public string PublishedScope { get; set; }

        public virtual ICollection<CrossPlatformProductListing> CrossPlatformProductListings { get; set; }
        public virtual ICollection<MctproductImage> MctproductImages { get; set; }
        public virtual ICollection<MctproductVariant> MctproductVariants { get; set; }
    }
}
