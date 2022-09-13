using CMS_EF.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS_EF.Configurations.Identity
{
    public class ApplicationActionConfiguration : IEntityTypeConfiguration<ApplicationAction>
    {
        public void Configure(EntityTypeBuilder<ApplicationAction> builder)
        {
            builder.HasIndex(b => b.Name);
            builder.IsMemoryOptimized();
        }
    }
}
