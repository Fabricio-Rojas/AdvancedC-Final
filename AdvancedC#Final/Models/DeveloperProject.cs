using AdvancedC_Final.Areas.Identity.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedC_Final.Models
{
    public class DeveloperProject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DeveloperId { get; set; }

        [ForeignKey("DeveloperId")]
        public TaskManagerUser Developer { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
    }
}
