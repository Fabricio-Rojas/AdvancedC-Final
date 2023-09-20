using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdvancedC_Final.Areas.Identity.Data;

namespace AdvancedC_Final.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Title is required.")]
        [MinLength(5, ErrorMessage = "Title must have at least 5 characters..")]
        [MaxLength(200, ErrorMessage = "Title has a limit of 200 characters.")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Project Manager Id")]
        public string ProjectManagerId { get; set; }

        [ForeignKey("ProjectManagerId")]
        public TaskManagerUser ProjectManager { get; set; }

        public HashSet<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
        public HashSet<DeveloperProject> Developers { get; set; } = new HashSet<DeveloperProject>();
    }
}