#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Customers
{
    public partial class CustomerCard
    {
        [Key]
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductSimilarId { get; set; }
        public int? Quantity { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedAt { get; set; }
        public int Flag { get; set; }
        public int? ProductId { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty("CustomerCard")]
        public virtual Customer Customer { get; set; }
    }
}