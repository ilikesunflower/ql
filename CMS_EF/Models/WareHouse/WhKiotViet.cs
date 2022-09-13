#nullable disable
using System;
using System.ComponentModel.DataAnnotations;

namespace CMS_EF.Models.WareHouse
{
    public partial class WhKiotViet
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string ClientId { get; set; }
        [StringLength(255)]
        public string ClientSecret { get; set; }
        
        public string PrefixApi { get; set; }
        public string Token { get; set; }
        public DateTime? ExpiredTokenTime { get; set; }
        
        public int? RetailerId { get; set; }
        public string Retailer { get; set; }
        public int? Status { get; set; }
        
        public int? BranchId { get; set; }
        
        public int? SoldById { get; set; }
        public int Flag { get; set; }
    }
}