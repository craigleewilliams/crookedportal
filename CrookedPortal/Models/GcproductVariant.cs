using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class GcproductVariant
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long? MctvariantId { get; set; }
        public long? EtsyProductId { get; set; }
        public long InventoryItemId { get; set; }
        public string InventoryPolicy { get; set; }
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public string Colour { get; set; }

        public virtual Gcproduct Product { get; set; }
    }
}
