using System.ComponentModel.DataAnnotations;

namespace LogViewer.Models
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
