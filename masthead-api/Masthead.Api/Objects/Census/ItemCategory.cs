using DbgCensus.Core.Objects;

namespace Masthead.Api.Objects.Census;

public record ItemCategory
(
    uint ItemCategoryId,
    GlobalizedString Name
);
