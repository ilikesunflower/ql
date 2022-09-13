#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Ship
{
    public partial class ShipVnPost
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string UserName { get; set; }
        [StringLength(255)]
        public string Password { get; set; }
        [StringLength(255)]
        public string CodePostal { get; set; }
        public bool? Status { get; set; }
        public int Flag { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        [StringLength(255)]
        public string PrefixApi { get; set; }
        [Column(TypeName = "ntext")]
        public string Token { get; set; }
        public string WardId { get; set; }
        public string ProvinceId { get; set; }
        public string DistrictId { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
    }
}