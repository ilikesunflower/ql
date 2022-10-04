#nullable disable
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Reports
{
    public partial class ReportAfterSales
    {
        [Key]
        public int Id { get; set; }
        [StringLength(4000)]
        public string Name { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string LinkFile { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? Type { get; set; }
        public int Flag { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Quater { get; set; }
    }
}