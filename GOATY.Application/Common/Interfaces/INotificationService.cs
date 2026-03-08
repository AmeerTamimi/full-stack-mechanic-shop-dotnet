using GOATY.Domain.Customers;

namespace GOATY.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task SendSmsAsync(Customer customer , CancellationToken ct);

        Task SendEmailAsync(Customer customer , CancellationToken ct);
    }
}
