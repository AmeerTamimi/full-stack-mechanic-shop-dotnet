namespace GOATY.Infrastructure.Settings
{
    public sealed class AppSettings
    {
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }
        public int MaxBays { get; set; }
        public int MinimumAppointmentDurationInMinutes { get; set; }
        public int LocalCacheExpirationInMins { get; set; }
        public int DistributedCacheExpirationMins { get; set; }
        public int DefaultPageNumber { get; set; }
        public int DefaultPageSize { get; set; }
        public int OverdueBookingCleanupMinutes { get; set; }
    }
}