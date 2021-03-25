using System.ComponentModel.DataAnnotations;

namespace GeneticWeb.Models
{
    public class ApplicationSettingModel
    {
        [Required]
        public string Application { set; get; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { set; get; }

        [Required]
        public string Value { set; get; }
    }
}
