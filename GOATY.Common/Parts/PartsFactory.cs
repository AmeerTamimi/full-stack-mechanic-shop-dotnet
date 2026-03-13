using GOATY.Domain.Common.Results;
using GOATY.Domain.Parts;

namespace GOATY.Tests.Common.Parts
{
    public static class PartFactory
    {
        public static Result<Part> Create(Guid? id = null,
                                          string? name = null,
                                          decimal? cost = null,
                                          int? quantity = null)
        {
            return Part.Create(
                id ?? Guid.NewGuid(),
                name ?? "PartName",
                cost ?? 1000m,
                quantity ?? 10);
        }
    }
}