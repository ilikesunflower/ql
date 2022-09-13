#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Articles
{
    public partial class ArticleType
    {
        public ArticleType()
        {
            Article = new HashSet<Article>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public int Flag { get; set; }
        [StringLength(255)]
        public string Alias { get; set; }

        [InverseProperty("ArticleTypeNavigation")]
        public virtual ICollection<Article> Article { get; set; }
    }
}