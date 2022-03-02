using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class MctconnectionDatum
    {
        public string Mctapikey { get; set; }
        public string Mctapiurl { get; set; }
        public long MctlocationId { get; set; }
        public string Mctpassword { get; set; }
    }
}
