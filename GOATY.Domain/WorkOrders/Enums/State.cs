using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOATY.Domain.WorkOrders.Enums
{
    public enum State
    {
        NoState = 1,
        Scheduled = 2,
        InProgress = 3,
        Finished = 4,
        Cancelled = 5
    }
}
