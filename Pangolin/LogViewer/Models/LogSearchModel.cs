using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LogViewer.Models
{
    public class LogSearchModel
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Source { set; get; }

        [Required]
        public DateTime BeginTime { set; get; }

        [Required]
        public DateTime EndTime { set; get; }
    }
}
