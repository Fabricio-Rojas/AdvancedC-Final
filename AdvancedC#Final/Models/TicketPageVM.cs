using X.PagedList;

namespace AdvancedC_Final.Models
{
    public class TicketPageVM
    {
        public Project Project { get; set; }
        public IPagedList<Ticket> Tickets { get; set; }
    }
}
