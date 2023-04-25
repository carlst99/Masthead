using DbgCensus.Core.Objects;

namespace Masthead.Api.Objects.GraphQLTypes;

public record Weapon
(
    int ItemId,
    int WeaponId,
    int ItemCategoryId,
    FactionDefinition Faction,
    string? Name,
    string? Description,
    string? ImagePath
);
