using GOATY.Domain.Common.Results;

namespace GOATY.Domain.Parts
{
    public static class PartErrors
    {
        public static readonly Error CostRequiredError = Error.Validation(
                                                            code: "Part.Cost.Required",
                                                            description: "Cost is required."
                                                        );

        public static readonly Error NameRequiredError = Error.Validation(
                                                            code: "Part.Name.Required",
                                                            description: "Name is required."
                                                        );

        public static readonly Error NameAlreadyExistsError = Error.Validation(
                                                            code: "Part.Name.Exists",
                                                            description: "Name already exists."
                                                        );

        public static readonly Error QuantityRequiredError = Error.Validation(
                                                                code: "Part.Quantity.Required",
                                                                description: "Quantity is required."
                                                            );
    }
}
