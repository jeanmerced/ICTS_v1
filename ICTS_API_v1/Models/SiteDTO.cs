using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    public class SiteDTO
    {
        //TODO: Unique
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_-]{1,30}", ErrorMessage = "Invalid: SiteName must contain only [a-zA-Z0-9_-].")]
        [StringLength(30)]
        public string SiteName { get; set; }

        // TODO: public TYPE Coordinates { get; set; }
    }
}
