#nullable disable
using System;
using System.ComponentModel.DataAnnotations;

namespace CMS_EF.Models.WareHouse
{
    public partial class WhTransaction
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? OrderIdKiot { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? TimeAtSync { get; set; }
    }
}