using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class DataTransfer
    {
        public int Id { get; set; }
        public int? MctproductsTransferred { get; set; }
        public int? GcproductsTransferred { get; set; }
        public int? EtsyListingsTransferred { get; set; }
        public DateTime DateOfTransfer { get; set; }
    }
}
