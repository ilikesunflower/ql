#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Orders
{
    public partial class OrderPartnerShipLog
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string PartnerShipCode { get; set; }
        public int? PartnerShipType { get; set; }
        [StringLength(255)]
        public string PartnerShipStatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PartnerShipCreatedAt { get; set; }
        [Column(TypeName = "ntext")]
        public string PartnerShipDetails { get; set; }
        public int? Status { get; set; }
        public string OrderCode { get; set; }
    }
}