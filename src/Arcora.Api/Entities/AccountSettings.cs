using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;

[Table("AccountSettings")]
public class AccountSettings
{
    [Key]
    public long UserID { get; set; }

    public bool EmailNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; }

    [MaxLength(20)]
    public string Language { get; set; } = "English";

    [MaxLength(100)]
    public string Timezone { get; set; } = "America/Regina";

    [MaxLength(3)]
    public string Currency { get; set; } = "CAD";

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserID))]
    public User? User { get; set; }
}
