using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace expenseTrackerPOC.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int? UserId { get; set; }
        [Required, MaxLength(50)]
        public string CategoryName { get; set; }
        public int IsDefault { get; set; } = 0;
        public int IconId { get; set; }
        public CategoryIcon Icon { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public User? User { get; set; }

    }
}