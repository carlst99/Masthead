using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Masthead.Api.Abstractions.Services;
using Masthead.Api.Objects.Census;
using Microsoft.Extensions.Caching.Memory;

namespace Masthead.Api.Services;

/// <summary>
/// <inheritdoc cref="ICensusRestService"/>
/// This class caches the results of calls made to the Census API.
/// </summary>
public class CachingCensusRestService : ICensusRestService
{
    private readonly ICensusRestService _censusRestService;
    private readonly IMemoryCache _cache;

    public CachingCensusRestService(ICensusRestService censusRestService, IMemoryCache cache)
    {
        _censusRestService = censusRestService;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WeaponItem>?> GetAllWeaponItemsAsync(CancellationToken ct = default)
    {
        return await _cache.GetOrCreateAsync
        (
            typeof(IReadOnlyList<WeaponItem>),
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                IReadOnlyList<WeaponItem>? items = await _censusRestService.GetAllWeaponItemsAsync(ct);

                if (items is not null)
                {
                    foreach (WeaponItem item in items)
                    {
                        _cache.Set
                        (
                            (typeof(WeaponItem), item.ItemId),
                            item,
                            TimeSpan.FromHours(24)
                        );
                    }
                }

                return items;
            }
        );
    }

    /// <inheritdoc />
    public async Task<WeaponItem?> GetWeaponItemByIdAsync(uint itemId, CancellationToken ct = default)
    {
        return await _cache.GetOrCreateAsync
        (
            (typeof(WeaponItem), itemId),
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                return await _censusRestService.GetWeaponItemByIdAsync(itemId, ct);
            }
        );
    }

    /// <inheritdoc />
    public async Task<WeaponStats?> GetWeaponStatsAsync(uint weaponId, CancellationToken ct = default)
    {
        return await _cache.GetOrCreateAsync
        (
            (typeof(WeaponStats), weaponId),
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                return await _censusRestService.GetWeaponStatsAsync(weaponId, ct);
            }
        );
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<uint, ItemCategory>?> GetAllItemCategoriesAsync(CancellationToken ct = default)
    {
        return await _cache.GetOrCreateAsync
        (
            typeof(ItemCategory),
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                return await _censusRestService.GetAllItemCategoriesAsync(ct);
            }
        );
    }
}
