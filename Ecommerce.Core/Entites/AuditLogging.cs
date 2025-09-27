

namespace Ecommerce.Core.Entites
{
    public class AuditLogging
    {
        public int Id { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

    }
}
