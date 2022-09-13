#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.Orders;
using CMS_EF.Models.PreOrders;

namespace CMS_EF.Models.Customers
{
    public partial class Customer
    {
        public Customer()
        {
            CustomerAddress = new HashSet<CustomerAddress>();
            CustomerCard = new HashSet<CustomerCard>();
            CustomerCoupon = new HashSet<CustomerCoupon>();
            CustomerPoint = new HashSet<CustomerPoint>();
            Orders = new HashSet<Orders.Orders>();
            PreOrder = new HashSet<PreOrder>();
            CustomerTracking = new HashSet<CustomerTracking>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string UserName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedAt { get; set; }
        public int? Status { get; set; }
        public int Flag { get; set; }
        public int Point { get; set; }
        public int? TypeGroup { get; set; }
        public int? Type { get; set; }
        [StringLength(255)]
        public string Password { get; set; }
        [StringLength(255)]
        public string Email { get; set; }
        [StringLength(255)]
        public string FullName { get; set; }
        
        [StringLength(255)]
        public string Phone { get; set; }
        
        [StringLength(255)]
        public string Detail { get; set; }
        
        [StringLength(255)]
        public string Org { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<CustomerAddress> CustomerAddress { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<CustomerCard> CustomerCard { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<CustomerCoupon> CustomerCoupon { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<CustomerPoint> CustomerPoint { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<Orders.Orders> Orders { get; set; }
        [InverseProperty("Customer")]
        public virtual ICollection<PreOrder> PreOrder { get; set; }
        
        [InverseProperty("Customer")]
        public virtual ICollection<OrderProductRateComment> OrderProductRateComment { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<CustomerTracking> CustomerTracking { get; set; }

    }
}