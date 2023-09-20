using AdvancedC_Final.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedC_Final.Models
{
    public class DeveloperTicket
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User Id is required.")]
        [Display(Name = "User Id")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public TaskManagerUser User { get; set; }

        [Required(ErrorMessage = "Ticked Id is required.")]
        [Display(Name = "Ticked Id")]
        public int TickedId { get; set; }

        [ForeignKey(nameof(TickedId))]
        public Ticket Ticket { get; set; }
    }
}
