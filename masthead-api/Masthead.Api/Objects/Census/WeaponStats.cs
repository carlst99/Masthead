using System.Collections.Generic;

namespace Masthead.Api.Objects.Census;

public record WeaponStats
(
    bool EquipNeedsAmmo,
    ushort EquipMs,
    ushort UnequipMs,
    ushort ToIronSightsMs,
    ushort FromIronSightsMs,
    ushort SprintRecoveryMs,
    uint NextUseDelayMs,
    decimal TurnModifier,
    decimal MoveModifier,
    ushort? HeatBleedOffRate,
    ushort? HeatOverheatPenaltyMs,
    decimal? MeleeDetectWidth,
    decimal? MeleeDetectHeight,
    bool RequiresAmmo,
    uint UseCooldownMs,
    IReadOnlyList<WeaponStats.WeaponAmmoSlot> AmmoSlots
)
{
    public record WeaponAmmoSlot
    (
        uint WeaponSlotIndex,
        short ClipSize,
        ushort Capacity
    );
}
