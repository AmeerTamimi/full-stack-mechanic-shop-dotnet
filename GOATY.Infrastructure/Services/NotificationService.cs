using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Customers;

using Microsoft.Extensions.Logging;

namespace GOATY.Infrastructure.Services
{
    public sealed class NotificationService(
        ILogger<NotificationService> logger)
        : INotificationService
    {
        public async Task SendEmailAsync(Customer customer, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                logger.LogWarning(
                    "Email notification was skipped because customer {CustomerId} has no email.",
                    customer.Id);

                return;
            }

            var subject = "Your work order has been completed";
            var body =
                $"Hello {customer.FullName}, your vehicle service has been completed successfully. Thank you for choosing GOATY.";

            await Task.Delay(300, ct); // hmmmm , so we're having a good faking process here LMAO

            logger.LogInformation(
                """
                [FAKE EMAIL]
                To: {Email}
                Subject: {Subject}
                Body: {Body}
                """,
                customer.Email,
                subject,
                body);
        }

        public async Task SendSmsAsync(Customer customer, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(customer.Phone))
            {
                logger.LogWarning(
                    "SMS notification was skipped because customer {CustomerId} has no phone number.",
                    customer.Id);

                return;
            }

            var message =
                $"Hello {customer.FullName}, your work order is complete. Thank you for choosing GOATY.";

            await Task.Delay(150, ct);

            logger.LogInformation(
                "[FAKE SMS] To: {Phone} | Message: {Message}",
                customer.Phone,
                message);
        }
    }
}