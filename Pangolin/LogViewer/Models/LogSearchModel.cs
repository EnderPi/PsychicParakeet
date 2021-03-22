using System;
using System.ComponentModel.DataAnnotations;

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
