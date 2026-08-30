using HomeLabCore.Domain.Entities.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeLabCore.Infrastructure.Database.Configurations;

internal sealed class MediaSubscriptionConfiguration : IEntityTypeConfiguration<MediaSubscription>
{
    public void Configure(EntityTypeBuilder<MediaSubscription> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.MediaExternalId, x.MediaType });
    }
}
