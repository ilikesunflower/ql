#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Ship
{
    [Table("ShipGHN")]
    public partial class ShipGhn
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Token { get; set; }
        [StringLength(255)]
        public string ShopId { get; set; }
        [StringLength(255)]
        public string ClientId { get; set; }
        [StringLength(255)]
        public string DistrictId { get; set; }
        public int Flag { get; set; }
        public bool? Status { get; set; }
        [StringLength(255)]
        public string PrefixApi { get; set; }
    }
}