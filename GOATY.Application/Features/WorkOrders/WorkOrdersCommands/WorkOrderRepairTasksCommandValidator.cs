using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands
{
    public sealed class WorkOrderRepairTasksCommandValidator : AbstractValidator<WorkOrderRepairTasksCommand>
    {
        public WorkOrderRepairTasksCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("RepairTaskId is required.");
        }
    }
}
