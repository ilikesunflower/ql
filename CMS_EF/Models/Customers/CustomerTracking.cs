#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Customers
{
    public class CustomerTracking
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime? ActiveTimeStart { get; set; }
        public DateTime? ActiveTimeEnd { get; set; }
        public int Flag { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty("CustomerTracking")]
        public virtual Customer Customer { get; set; }
    }
}