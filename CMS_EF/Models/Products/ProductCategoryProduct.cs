#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Products
{
    public partial class ProductCategoryProduct
    {
        [Key]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        [Column("PCategoryId")]
        public int? PcategoryId { get; set; }
        public int Flag { get; set; }
        public int? Ord { get; set; }

        [ForeignKey("PcategoryId")]
        [InverseProperty("ProductCategoryProduct")]
        public virtual ProductCategory Pcategory { get; set; }
        [ForeignKey("ProductId")]
        [InverseProperty("ProductCategoryProduct")]
        public virtual Products Product { get; set; }
    }
}