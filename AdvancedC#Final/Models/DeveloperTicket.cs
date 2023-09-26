using AdvancedC_Final.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedC_Final.Models
{
    public class DeveloperTicket
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User Id is required.")]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public TaskManagerUser User { get; set; }

        [Required(ErrorMessage = "Ticket Id is required.")]
        [ForeignKey(nameof(Ticket))]
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}
