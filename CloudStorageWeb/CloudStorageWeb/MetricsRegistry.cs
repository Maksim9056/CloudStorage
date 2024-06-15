using Prometheus;
namespace CloudStorageWeb
{

    public static class MetricsRegistry
    {
        public static readonly Counter RequestsCounter = Metrics.CreateCounter(
            "myapp_requests_total",
            "Number of requests processed by the application"
        );

        public static void IncrementRequestsCount()
        {
            RequestsCounter.Inc();
        }
    }

}
