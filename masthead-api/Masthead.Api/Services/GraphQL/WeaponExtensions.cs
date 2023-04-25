using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using Masthead.Api.Abstractions.Services;
using Masthead.Api.Objects.Census;
using Masthead.Api.Objects.GraphQLTypes;

namespace Masthead.Api.Services.GraphQL;

[ExtendObjectType<Weapon>]
public class WeaponExtensions
{
    public async Task<string?> GetItemCategoryNameAsync
    (
        [Parent] Weapon weapon,
        [Service] ICensusRestService censusRestService,
        CancellationToken ct
    )
    {
        return (await censusRestService.GetAllItemCategoriesAsync(ct))?
            .TryGetValue((uint)weapon.ItemCategoryId, out ItemCategory? value) is true
                ? value.Name.English.GetValueOrDefault()
                : null;
    }
}
