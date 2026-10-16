
namespace RaceDay.API.Models
{
    public partial class Event
    {
        public int EventId { get; set; }

        public int OrganiserId { get; set; }

        public string EventName { get; set; } = null!;

        public string? Description { get; set; }

        public string EventType { get; set; } = null!;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = null!;

        public decimal EntryFee { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual User Organiser { get; set; } = null!;

        public virtual ICollection<Category> Categories { get; set; }
            = new List<Category>();

        public virtual ICollection<Enrolment> Enrolments { get; set; }
            = new List<Enrolment>();

        public virtual ICollection<RaceDay.API.Models.Route> Routes { get; set; }
            = new List<RaceDay.API.Models.Route>();
    }
}

