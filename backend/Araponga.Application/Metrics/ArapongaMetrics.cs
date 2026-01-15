using System.Diagnostics.Metrics;

namespace Araponga.Application.Metrics;

/// <summary>
/// Centralized metrics for Araponga application using .NET Metrics API.
/// </summary>
public static class ArapongaMetrics
{
    private static readonly Meter Meter = new("Araponga", "1.0.0");

    // Business Metrics
    public static readonly Counter<long> PostsCreated = Meter.CreateCounter<long>(
        "araponga.posts.created",
        "count",
        "Total number of posts created");

    public static readonly Counter<long> EventsCreated = Meter.CreateCounter<long>(
        "araponga.events.created",
        "count",
        "Total number of events created");

    public static readonly Counter<long> MembershipsCreated = Meter.CreateCounter<long>(
        "araponga.memberships.created",
        "count",
        "Total number of memberships created");

    public static readonly Counter<long> TerritoriesCreated = Meter.CreateCounter<long>(
        "araponga.territories.created",
        "count",
        "Total number of territories created");

    public static readonly Counter<long> ReportsCreated = Meter.CreateCounter<long>(
        "araponga.reports.created",
        "count",
        "Total number of moderation reports created");

    public static readonly Counter<long> JoinRequestsCreated = Meter.CreateCounter<long>(
        "araponga.join_requests.created",
        "count",
        "Total number of join requests created");

    // Cache Metrics
    public static readonly Counter<long> CacheHits = Meter.CreateCounter<long>(
        "araponga.cache.hits",
        "count",
        "Total number of cache hits");

    public static readonly Counter<long> CacheMisses = Meter.CreateCounter<long>(
        "araponga.cache.misses",
        "count",
        "Total number of cache misses");

    // Concurrency Metrics
    public static readonly Counter<long> ConcurrencyConflicts = Meter.CreateCounter<long>(
        "araponga.concurrency.conflicts",
        "count",
        "Total number of concurrency conflicts detected");

    // Event Processing Metrics
    public static readonly Counter<long> EventsProcessed = Meter.CreateCounter<long>(
        "araponga.events.processed",
        "count",
        "Total number of events processed");

    public static readonly Counter<long> EventsFailed = Meter.CreateCounter<long>(
        "araponga.events.failed",
        "count",
        "Total number of events that failed processing");

    public static readonly Histogram<double> EventProcessingDuration = Meter.CreateHistogram<double>(
        "araponga.events.processing.duration",
        "ms",
        "Duration of event processing in milliseconds");

    // Database Metrics
    public static readonly Histogram<double> DatabaseQueryDuration = Meter.CreateHistogram<double>(
        "araponga.database.query.duration",
        "ms",
        "Duration of database queries in milliseconds");
}
