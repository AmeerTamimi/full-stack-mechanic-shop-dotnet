namespace GOATY.Contracts.Requests
{
    public sealed class PartRequirementsRequest
    {
        public Guid PartId { get; set; }
        public int Quantity { get; set; }
    }
}
