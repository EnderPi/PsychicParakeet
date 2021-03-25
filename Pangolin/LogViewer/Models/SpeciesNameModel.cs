using System.ComponentModel.DataAnnotations;

namespace GeneticWeb.Models
{
    public class SpeciesNameModel
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { set; get; }
    }
}
