using Domain.Enums;

namespace Domain.Entities
{
    public partial class Companion
    {
        public int Id { get; set; }
        public CompanionRelationship Relationship { get; set; }
        public int PersonId { get; set; }
        public int UserId { get; set; }
        public Person Person { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Companion_Booking> CompanionBookings { get; set; } = new List<Companion_Booking>();
    }
}
