using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using Masthead.Api.Abstractions.Services;
using Masthead.Api.Objects.Census;
using Microsoft.Extensions.Logging;

namespace Masthead.Api.Services;

/// <inheritdoc />
public class CensusRestService : ICensusRestService
{
    private readonly ILogger<CensusRestService> _logger;
    private readonly IQueryService _queryService;

    public CensusRestService
    (
        ILogger<CensusRestService> logger,
        IQueryService queryService
    )
    {
        _logger = logger;
        _queryService = queryService;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WeaponItem>?> GetAllWeaponItemsAsync(CancellationToken ct = default)
    {
        IQueryBuilder query = _queryService.CreateQuery()
            .OnCollection("item_to_weapon")
            .WithLimit(5000)
            .AddJoin("item", j =>
            {
                j.ShowFields("item_category_id", "faction_id", "name.en", "description.en", "image_path")
                    .InjectAt("item_details")
                    .IsInnerJoin();
            });

        return await GetAsync<IReadOnlyList<WeaponItem>>(query, ct);
    }

    /// <inheritdoc />
    public async Task<WeaponItem?> GetWeaponItemByIdAsync(uint itemId, CancellationToken ct = default)
    {
        IQueryBuilder query = _queryService.CreateQuery()
            .OnCollection("item_to_weapon")
            .Where("item_id", SearchModifier.Equals, itemId)
            .AddJoin("item", j =>
            {
                j.ShowFields("item_category_id", "faction_id", "name.en", "description.en", "image_path")
                    .InjectAt("item_details")
                    .IsInnerJoin();
            });

        return await GetAsync<WeaponItem>(query, ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<uint, ItemCategory>?> GetAllItemCategoriesAsync(CancellationToken ct = default)
    {
        IQueryBuilder query = _queryService.CreateQuery()
            .OnCollection("item_category")
            .WithLimit(500);

        IReadOnlyList<ItemCategory>? categories =  await GetAsync<IReadOnlyList<ItemCategory>>(query, ct);
        return categories?.ToDictionary(x => x.ItemCategoryId, x => x);
    }

    /// <inheritdoc />
    public async Task<WeaponStats?> GetWeaponStatsAsync(uint weaponId, CancellationToken ct = default)
    {
        IQueryBuilder query = _queryService.CreateQuery()
            .OnCollection("weapon")
            .Where("weapon_id", SearchModifier.Equals, weaponId)
            .HideFields
            (
                "to_iron_sights_anim_ms",
                "from_iron_sights_anim_ms",
                "range_description",
                "animation_weirld_type_name",
                "min_view_pitch",
                "max_view_pitch"
            )
            .AddJoin("weapon_ammo_slot", j =>
            {
                j.ShowFields("weapon_slot_index", "clip_size", "capacity")
                    .IsList()
                    .InjectAt("ammo_slots");
            });

        return await GetAsync<WeaponStats>(query, ct);
    }

    private async Task<T?> GetAsync<T>
    (
        IQueryBuilder query,
        CancellationToken ct,
        [CallerMemberName] string? caller = null
    )
    {
        try
        {
            return await _queryService.GetAsync<T>(query, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Census call from {Caller} failed", caller);
            return default;
        }
    }
}
