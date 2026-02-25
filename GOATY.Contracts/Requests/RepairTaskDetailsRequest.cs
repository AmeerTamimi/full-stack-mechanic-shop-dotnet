namespace GOATY.Contracts.Requests
{
    public sealed class RepairTaskDetailsRequest
    {
        public Guid PartId { get; set; }
        public int Quantity { get; set; }
    }
}
