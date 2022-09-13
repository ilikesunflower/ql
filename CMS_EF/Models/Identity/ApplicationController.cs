using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CMS_EF.Models.Identity
{
    public partial class ApplicationController
    {

        public int Id { get; set; }

        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        [DefaultValue("(getdate())")]
        public DateTime CreatedAt { get; set; }

        [DefaultValue(0)]
        public int CreatedBy { get; set; }

        [DefaultValue(0)]
        public int Flag { get; set; }
        public virtual ICollection<ApplicationAction> ApplicationActions { get; set; }
    }
}
