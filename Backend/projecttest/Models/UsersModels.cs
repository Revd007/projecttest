using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace projecttest.Models
{
    [Table("users")]
    public class UsersModels
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("phone_number")]
        public string PhoneNumber {  get; set; } = string.Empty;

        [Column("password_hash")]
        public string PasswordHash {  get; set; } = string.Empty;

        [Column("email_verified")]
        public bool EmailVerified { get; set; } = false;

        [Column("phone_verified")]
        public bool PhoneVerified { get; set; } = false;

        [Column("verification_token")]
        public string? VerificationToken { get; set; }

        [Column("token_expiry")]
        public DateTime? TokenExpiry { get; set; }
    }
}
