#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.Categories;

namespace CMS_EF.Models.PreOrders
{
    public class PreOrderAddress
    {
        [Key] 
        public int Id { get; set; }
        public int PreOrderId { get; set; }
        
        [StringLength(255)] 
        public string ProvinceCode { get; set; }
        
        [StringLength(255)] 
        public string DistrictCode { get; set; }
        
        [StringLength(255)] 
        public string CommuneCode { get; set; }
        
        [StringLength(255)] 
        public string Address { get; set; }
        
        [StringLength(255)] 
        public string Name { get; set; }
        
        [StringLength(255)] 
        public string Phone { get; set; }
        
        [StringLength(255)] 
        public string Email { get; set; }
        
        [StringLength(255)] 
        public string Note { get; set; }
        
        public int Flag { get; set; }
        
        public DateTime? LastModifiedAt { get; set; }

        [ForeignKey("PreOrderId")]
        [InverseProperty("PreOrderAddress")]
        public virtual PreOrder PreOrder { get; set; }
        
        [ForeignKey("ProvinceCode")]
        public virtual Province Province { get; set; }
        
        [ForeignKey("DistrictCode")]
        public virtual District District { get; set; } 
        
        [ForeignKey("CommuneCode")]
        public virtual Commune Commune { get; set; } 
    }
}