namespace GOATY.Contracts.Requests
{
    public sealed class PartRequest
    {
        public string? Name { get; set; }
        public decimal Cost { get; set; }
        public int Quantity { get; set; }
    }
}
