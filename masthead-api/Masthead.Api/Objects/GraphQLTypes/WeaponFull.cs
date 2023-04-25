using System.Collections.Generic;
using DbgCensus.Core.Objects;

namespace Masthead.Api.Objects.GraphQLTypes;

public record WeaponFull
(
    int ItemId,
    int WeaponId,
    int ItemCategoryId,
    FactionDefinition Faction,
    string? Name,
    string? Description,
    string? ImagePath,
    bool EquipNeedsAmmo,
    short EquipMs,
    short UnequipMs,
    short ToIronSightsMs,
    short FromIronSightsMs,
    short SprintRecoveryMs,
    int NextUseDelayMs,
    decimal TurnModifier,
    decimal MoveModifier,
    short? HeatBleedOffRate,
    short? HeatOverheatPenaltyMs,
    decimal? MeleeDetectWidth,
    decimal? MeleeDetectHeight,
    bool RequiresAmmo,
    int UseCooldownMs,
    IEnumerable<WeaponFull.AmmoSlot> AmmoSlots
) : Weapon(ItemId, WeaponId, ItemCategoryId, Faction, Name, Description, ImagePath)
{
    public record AmmoSlot
    (
        int WeaponSlotIndex,
        short ClipSize,
        short Capacity
    );
}
