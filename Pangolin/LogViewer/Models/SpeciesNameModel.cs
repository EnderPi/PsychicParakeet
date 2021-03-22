using System.ComponentModel.DataAnnotations;

namespace LogViewer.Models
{
    public class SpeciesNameModel
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { set; get; }
    }
}
