#nullable disable
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Products
{
    public partial class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [StringLength(255)]
        public string Link { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public int Flag { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("ProductImage")]
        public virtual Products Product { get; set; }
    }
}