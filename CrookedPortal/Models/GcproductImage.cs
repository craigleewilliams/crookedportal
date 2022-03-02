using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class GcproductImage
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public int Position { get; set; }
        public string Url { get; set; }
        public byte[] Image { get; set; }

        public virtual Gcproduct Product { get; set; }
    }
}
