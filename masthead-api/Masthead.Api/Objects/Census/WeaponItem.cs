using DbgCensus.Core.Objects;

namespace Masthead.Api.Objects.Census;

public record WeaponItem
(
    uint ItemId,
    uint WeaponId,
    WeaponItem.Item? ItemDetails
)
{
    public record Item
    (
        uint ItemCategoryId,
        FactionDefinition FactionId,
        GlobalizedString? Name,
        GlobalizedString? Description,
        string? ImagePath
    );
}
