using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdvancedC_Final.Areas.Identity.Data;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;

namespace AdvancedC_Final.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MinLength(5, ErrorMessage = "Title must have at least 5 characters.")]
        [MaxLength(200, ErrorMessage = "Title has a limit of 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Priority is required.")]
        public Priority Priority { get; set; }

        [Required(ErrorMessage = "Required Hours is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Cannot asign negative required hours")]
        [DisplayName("Required Hours")]
        public int RequiredHours { get; set; }

        [Required(ErrorMessage = "Completed is required.")]
        [DisplayName("Completed")]
        public bool IsCompleted { get; set; }

        [Required(ErrorMessage = "Project Id is required.")]
        [DisplayName("Project Id")]
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        public HashSet<DeveloperTicket> Developers { get; set; } = new HashSet<DeveloperTicket>();
    }
    public enum Priority
    {
        Low,
        Medium,
        High
    }
}
