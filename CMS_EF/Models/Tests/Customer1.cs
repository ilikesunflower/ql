#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CMS_EF.Models.Tests
{
    public partial class Customer1
    {
        [Column("createdAt")]
        public long? CreatedAt { get; set; }
        [Column("updatedAt")]
        public long? UpdatedAt { get; set; }
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }
        [Column("phone")]
        [StringLength(255)]
        public string Phone { get; set; }
        [Column("facebookId")]
        [StringLength(255)]
        public string FacebookId { get; set; }
        [Column("googleId")]
        [StringLength(255)]
        public string GoogleId { get; set; }
        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; }
        [Column("birth")]
        public double? Birth { get; set; }
        [Column("gender")]
        [StringLength(20)]
        public string Gender { get; set; }
        [Column("address")]
        [StringLength(255)]
        public string Address { get; set; }
        [Column("avatar")]
        [StringLength(255)]
        public string Avatar { get; set; }
        [Column("contact")]
        [StringLength(255)]
        public string Contact { get; set; }
        [Column("provinceId")]
        public int? ProvinceId { get; set; }
        [Column("districtId")]
        public int? DistrictId { get; set; }
        [Column("wardId")]
        public int? WardId { get; set; }
        [Column("inviteCode")]
        [StringLength(255)]
        public string InviteCode { get; set; }
        [Column("secure")]
        [StringLength(255)]
        public string Secure { get; set; }
        [Column("point")]
        public double? Point { get; set; }
        [Column("scope")]
        [StringLength(255)]
        public string Scope { get; set; }
        [Column("username")]
        [StringLength(255)]
        public string Username { get; set; }
        [Column("refId")]
        [StringLength(255)]
        public string RefId { get; set; }
        [Column("agencyId")]
        [StringLength(255)]
        public string AgencyId { get; set; }
        [Column("companyTaxName")]
        [StringLength(255)]
        public string CompanyTaxName { get; set; }
        [Column("adrressTax")]
        [StringLength(255)]
        public string AdrressTax { get; set; }
        [Column("taxNumber")]
        [StringLength(255)]
        public string TaxNumber { get; set; }
        [Column("receiveEmailTax")]
        [StringLength(255)]
        public string ReceiveEmailTax { get; set; }
    }
}