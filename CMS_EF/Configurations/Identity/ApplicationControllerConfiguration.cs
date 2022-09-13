using CMS_EF.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS_EF.Configurations.Identity
{
    public class ApplicationControllerConfiguration : IEntityTypeConfiguration<ApplicationController>
    {
        public void Configure(EntityTypeBuilder<ApplicationController> builder)
        {
            builder.HasIndex(b => b.Name);
            builder.IsMemoryOptimized();
        }
    }
}
