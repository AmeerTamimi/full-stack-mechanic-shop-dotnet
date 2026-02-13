using GOATY.Domain.Common.Results;

namespace GOATY.Domain.Parts
{
    public static class PartErrors
    {
        public static readonly Error CostInvalidError = Error.Validation(
                                                            code: "Part.Cost.InValid",
                                                            description: "Cost must be > 0."
                                                        );

        public static readonly Error NameInvalidError = Error.Validation(
                                                            code: "Part.Name.InValid",
                                                            description: "Name must have Value."
                                                        );

        public static readonly Error QuantityInvalidError = Error.Validation(
                                                                code: "Part.Quantity.InValid",
                                                                description: "Quantity must be > 0."
                                                            );
    }
}
