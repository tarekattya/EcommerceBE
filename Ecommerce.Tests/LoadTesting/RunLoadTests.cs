
namespace Ecommerce.Tests.LoadTesting;
public class RunLoadTests
{
    public static async Task Main(string[] args)
    {
        var baseUrl = args.Length > 0 ? args[0] : "https://localhost:7021";
        Console.WriteLine($"Running load tests against: {baseUrl}");
        Console.WriteLine("========================================\n");

        var runner = new LoadTestRunner(baseUrl);
        try
        {

        // Light Load Test
        Console.WriteLine("1. Running Light Load Test (10 concurrent requests)...");
        var lightLoad = await runner.RunLoadTestAsync(
            "/api/dashboard/stats",
            concurrentRequests: 10,
            requestsPerClient: 1
        );
        lightLoad.PrintReport();

        // Medium Load Test
        Console.WriteLine("\n2. Running Medium Load Test (50 concurrent, 2 requests each)...");
        var mediumLoad = await runner.RunLoadTestAsync(
            "/api/dashboard/stats",
            concurrentRequests: 50,
            requestsPerClient: 2
        );
        mediumLoad.PrintReport();

        // Heavy Load Test
        Console.WriteLine("\n3. Running Heavy Load Test (100 concurrent, 5 requests each)...");
        var heavyLoad = await runner.RunLoadTestAsync(
            "/api/dashboard/stats",
            concurrentRequests: 100,
            requestsPerClient: 5
        );
        heavyLoad.PrintReport();

        Console.WriteLine("\nLoad testing completed!");
        }
        finally
        {
            runner.Dispose();
        }
    }
}
