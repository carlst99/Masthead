using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using Masthead.Api.Abstractions.Services;
using Masthead.Api.Objects.Census;
using Masthead.Api.Objects.GraphQLTypes;

namespace Masthead.Api.Services.GraphQL;

public class GraphQLQueryService
{
    public async Task<IEnumerable<Weapon>> GetWeaponsAsync
    (
        [Service] ICensusRestService censusRestService,
        CancellationToken ct
    )
    {
        IReadOnlyList<WeaponItem>? weaponItems = await censusRestService.GetAllWeaponItemsAsync(ct);
        if (weaponItems is null)
            return Array.Empty<Weapon>();

        return weaponItems.Where(x => x.ItemDetails is not null)
            .Select(x => new Weapon
            (
                (int)x.ItemId,
                (int)x.WeaponId,
                (int)x.ItemDetails!.ItemCategoryId,
                x.ItemDetails.FactionId,
                x.ItemDetails.Name?.English.GetValueOrDefault(),
                x.ItemDetails.Description?.English.GetValueOrDefault(),
                x.ItemDetails.ImagePath
            ));
    }

    public async Task<WeaponFull?> GetWeaponAsync
    (
        int weaponItemId,
        [Service] ICensusRestService censusRestService,
        CancellationToken ct
    )
    {
        WeaponItem? weapon = await censusRestService.GetWeaponItemByIdAsync((uint)weaponItemId, ct);
        if (weapon is null)
            return null;

        WeaponStats? stats = await censusRestService.GetWeaponStatsAsync(weapon.WeaponId, ct);
        if (stats is null)
            return null;

        return new WeaponFull
        (
            (int)weapon.ItemId,
            (int)weapon.WeaponId,
            (int)weapon.ItemDetails!.ItemCategoryId,
            weapon.ItemDetails.FactionId,
            weapon.ItemDetails.Name?.English.GetValueOrDefault(),
            weapon.ItemDetails.Description?.English.GetValueOrDefault(),
            weapon.ItemDetails.ImagePath,
            stats.EquipNeedsAmmo,
            (short)stats.EquipMs,
            (short)stats.UnequipMs,
            (short)stats.ToIronSightsMs,
            (short)stats.FromIronSightsMs,
            (short)stats.SprintRecoveryMs,
            (int)stats.NextUseDelayMs,
            stats.TurnModifier,
            stats.MoveModifier,
            (short?)stats.HeatBleedOffRate,
            (short?)stats.HeatOverheatPenaltyMs,
            stats.MeleeDetectWidth,
            stats.MeleeDetectHeight,
            stats.RequiresAmmo,
            (int)stats.UseCooldownMs,
            stats.AmmoSlots.Select(x => new WeaponFull.AmmoSlot
            (
                (int)x.WeaponSlotIndex,
                x.ClipSize,
                (short)x.Capacity
            ))
        );
    }
}
