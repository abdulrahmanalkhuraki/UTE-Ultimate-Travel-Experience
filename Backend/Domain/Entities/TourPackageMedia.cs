using System;

namespace Domain.Entities
{
    public partial class TourPackageMedia : BaseEntity
    {
        public int TourPackageId { get; set; }
        public string MediaUrl { get; set; } = null!;
        public Domain.Enums.MediaType MediaType { get; set; }
        public int DisplayOrder { get; set; }

        // Navigation
        public virtual TourPackage TourPackage { get; set; } = null!;
    }
}
