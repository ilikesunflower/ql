#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Customers
{
    public class HistoryFileChargePoint
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string Code { get; set; }
        [Column(TypeName = "ntext")]
        public string Note { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        [StringLength(255)]
        public string LinkFile { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }
        
        [StringLength(255)]
        public string ReleaseBy { get; set; }
        
        public int? IsSentNotification { get; set; }

        public int Flag { get; set; }
    }
}