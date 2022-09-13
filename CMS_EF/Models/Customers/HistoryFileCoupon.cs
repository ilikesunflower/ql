#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Customers
{
    public partial class HistoryFileCoupon
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [Column(TypeName = "ntext")]
        public string Detail { get; set; }
        [StringLength(255)]
        public string LinkFile { get; set; }
        [StringLength(255)]
        public string Code { get; set; }
        public int Flag { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }  
        [StringLength(255)]
        public string OrgName { get; set; }
        public bool? IsSentNotification { get; set; }
    }
}