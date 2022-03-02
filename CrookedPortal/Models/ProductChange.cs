using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class ProductChange
    {
        public int Id { get; set; }
        public long ProductId { get; set; }
        public string Operation { get; set; }
    }
}
