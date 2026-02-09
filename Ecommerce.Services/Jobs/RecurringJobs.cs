using Hangfire;
using Hangfire.Common;

namespace Ecommerce.Services;

public static class RecurringJobs
{
    /// <summary>
    /// Registers recurring jobs using service-based API (IRecurringJobManager).
    /// Call after the app is built so Hangfire JobStorage is initialized.
    /// </summary>
    public static void RegisterRecurringJobs(IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate(
            "daily-cleanup",
            Job.FromExpression<DailyCleanupJob>(x => x.Execute()),
            Cron.Daily(2, 0),
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });

    }
}
