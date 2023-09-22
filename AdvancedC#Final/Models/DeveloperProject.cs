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

        [Required(ErrorMessage = "Developer Id is required.")]
        [ForeignKey(nameof(Developer))]
        public string DeveloperId { get; set; }
        public TaskManagerUser Developer { get; set; }

        [Required(ErrorMessage = "Project Id is required.")]
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
