using System.ComponentModel.DataAnnotations;

namespace GeneticWeb.Models
{
    public class GlobalSettingModel
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { set; get; }

        [Required]
        public string Value { set; get; }
    }
}
