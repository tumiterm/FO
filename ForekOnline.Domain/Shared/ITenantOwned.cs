namespace ForekOnline.Domain.Shared;

/// <summary>Marks a record whose data is owned by exactly one tenant.</summary>
public interface ITenantOwned
{
    Guid TenantId { get; set; }
}
