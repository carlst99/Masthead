using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Masthead.Api.Objects.Census;

namespace Masthead.Api.Abstractions.Services;

public interface ICensusRestService
{
    Task<IReadOnlyList<WeaponItem>?> GetAllWeaponItemsAsync(CancellationToken ct = default);
    Task<WeaponItem?> GetWeaponItemByIdAsync(uint itemId, CancellationToken ct = default);
    Task<IReadOnlyDictionary<uint, ItemCategory>?> GetAllItemCategoriesAsync(CancellationToken ct = default);
    Task<WeaponStats?> GetWeaponStatsAsync(uint weaponId, CancellationToken ct = default);
}
