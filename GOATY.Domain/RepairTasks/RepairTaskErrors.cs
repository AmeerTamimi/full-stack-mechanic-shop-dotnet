using GOATY.Domain.Common.Results;

namespace GOATY.Domain.RepairTasks
{
    public sealed class RepairTaskErrors
    {
        public static readonly Error InvalidId = Error.Validation(
                                                        code: "RepairTask.Id.Invalid",
                                                        description: "RepairTask id must not be empty."
                                                    );

        public static readonly Error InvalidName = Error.Validation(
                                                        code: "RepairTask.Name.Invalid",
                                                        description: "RepairTask name must have a value."
                                                    );

        public static readonly Error InvalidDescription = Error.Validation(
                                                        code: "RepairTask.Description.Invalid",
                                                        description: "RepairTask description must have a value."
                                                    );

        public static readonly Error InvalidRepairTask = Error.Validation(
                                                        code: "RepairTask.Details.Invalid",
                                                        description: "RepairTask must contain at least one detail."
                                                    );

        public static Error InvalidCostEstimated(decimal totalCost) => Error.Validation(
                                                        code: "RepairTask.Cost.Invalid",
                                                        description: $"Estimated cost cannot be less than {totalCost}."
                                                    );

        public static readonly Error InvalidTimeEstimated = Error.Validation(
                                                        code: "RepairTask.Time.Invalid",
                                                        description: "Invalid repair duration."
                                                    );
        
        public static readonly Error InvalidRepairTaskDetails = Error.Validation(
                                                        code: "RepairTask.RepairTaskDetails.Invalid",
                                                        description: "Invalid RepairTaskDetails List."
                                                    );

        public static readonly Error InvalidPartId = Error.Validation(
                                                        code: "RepairTask.Details.PartId.Invalid",
                                                        description: "Invalid RepairTaskDetails : PartId Is Invalid."
                                                    );

        public static readonly Error InvalidQuantity = Error.Validation(
                                                        code: "RepairTask.Details.Quantity.Invalid",
                                                        description: "Invalid RepairTaskDetails : Quantity Is Invalid."
                                                    );

        public static readonly Error InvalidUnitPrice = Error.Validation(
                                                        code: "RepairTask.Details.UnitPrice.Invalid",
                                                        description: "Invalid RepairTaskDetails : UnitPrice Is Invalid."
                                                    );

        public static readonly Error InvalidTechnicianCost = Error.Validation(
                                                        code: "RepairTask.TechnicianCost.Invalid",
                                                        description: "Invalid Technician Cost : Technician Cost Must Be >= 50."
                                                    );
        
    }
}
