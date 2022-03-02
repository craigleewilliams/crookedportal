using System;
using System.Collections.Generic;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class ErrorLog
    {
        public int Id { get; set; }
        public DateTime DateOfError { get; set; }
        public string Message { get; set; }
    }
}
