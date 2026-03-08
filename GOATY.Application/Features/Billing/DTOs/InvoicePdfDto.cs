namespace GOATY.Application.Features.Billing.DTOs
{
    public sealed class InvoicePdfDto
    {
        public byte[]? Content { get; init; } = [];
        public string? FileName { get; init; }
        public string? ContentType { get; set; } = "application/pdf";
    }
}
