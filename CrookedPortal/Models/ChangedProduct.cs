using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class ChangedProduct
    {
        public int Id { get; set; }
        public long ProductId { get; set; }
        public string Message { get; set; }
    }
}
