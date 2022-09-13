#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Orders
{
    public partial class OrderLog
    {
        [Key] public int Id { get; set; }
        public int? OrderId { get; set; }
        [Column(TypeName = "ntext")] public string Note { get; set; }
        [Column(TypeName = "datetime")] public DateTime? LastModifiedAt { get; set; }
        public int Flag { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("OrderLog")]
        public virtual Orders Order { get; set; }
    }
}