using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalorieTrackerService.Domain.Model.Entity
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("user_email")]
        public string UserEmail { get; set; } = string.Empty;

        [Column("user_password")]
        public byte[] UserPassword { get; set; } = Array.Empty<byte>();

        [Column("user_name")]
        public string UserName { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}