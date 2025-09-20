using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Inventory.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime CreatedAt { get; set; }

        // Foreign key
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
