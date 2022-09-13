#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Articles
{
    public partial class Article
    {
        [Key]
        public int Id { get; set; }
        public int ArticleType { get; set; }
        public string Title { get; set; }
        public string Lead { get; set; }
        [Column(TypeName = "ntext")]
        public string Detail { get; set; }
        [Column("video")]
        [StringLength(1)]
        public string Video { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PublishTime { get; set; }
        [StringLength(255)]
        public string Thumbnail { get; set; }
        [StringLength(255)]
        public string Author { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public int Flag { get; set; }
        public int? Status { get; set; }
        public bool? IsHot { get; set; }

        [ForeignKey("ArticleType")]
        [InverseProperty("Article")]
        public virtual ArticleType ArticleTypeNavigation { get; set; }
    }
}