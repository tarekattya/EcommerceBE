
namespace Ecommerce.Tests.LoadTesting;

public class DashboardLoadTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly LoadTestRunner _loadTestRunner;

    public DashboardLoadTests(ITestOutputHelper output)
    {
        _output = output;
        // Update with your actual API URL
        _loadTestRunner = new LoadTestRunner("https://localhost:7021");
    }

    [Fact]
    public async Task DashboardEndpoint_ShouldHandleLightLoad()
    {
        // Arrange & Act
        var result = await _loadTestRunner.RunLoadTestAsync(
            "/api/dashboard/stats",
            concurrentRequests: 10,
            requestsPerClient: 1
        );

        // Assert
        result.SuccessfulRequests.Should().BeGreaterThan(0);
        result.SuccessRate.Should().BeGreaterThan(0.9); // 90% success rate
        result.AverageResponseTime.TotalMilliseconds.Should().BeLessThan(1000); // Under 1 second
        
        _output.WriteLine($"Light Load Test: {result.SuccessfulRequests}/{result.TotalRequests} successful");
        result.PrintReport();
    }

    [Fact]
    public async Task DashboardEndpoint_ShouldHandleMediumLoad()
    {
        // Arrange & Act
        var result = await _loadTestRunner.RunLoadTestAsync(
            "/api/dashboard/stats",
            concurrentRequests: 50,
            requestsPerClient: 2
        );

        // Assert
        result.SuccessfulRequests.Should().BeGreaterThan(80); // At least 80% success
        result.AverageResponseTime.TotalMilliseconds.Should().BeLessThan(2000); // Under 2 seconds
        
        _output.WriteLine($"Medium Load Test: {result.SuccessfulRequests}/{result.TotalRequests} successful");
        result.PrintReport();
    }

    [Fact]
    public async Task DashboardEndpoint_ShouldHandleHeavyLoad()
    {
        // Arrange & Act
        var result = await _loadTestRunner.RunLoadTestAsync(
            "/api/dashboard/stats",
            concurrentRequests: 100,
            requestsPerClient: 5
        );

        // Assert
        result.SuccessfulRequests.Should().BeGreaterThan(0);
        // Heavy load may have some failures, but should still handle most requests
        result.SuccessRate.Should().BeGreaterThan(0.7); // At least 70% success under heavy load
        
        _output.WriteLine($"Heavy Load Test: {result.SuccessfulRequests}/{result.TotalRequests} successful");
        result.PrintReport();
    }

    [Fact]
    public async Task DashboardEndpoint_ShouldMaintainPerformance_UnderSustainedLoad()
    {
        // Arrange & Act - Sustained load over time
        var results = new List<LoadTestResult>();
        
        for (int i = 0; i < 5; i++)
        {
            var result = await _loadTestRunner.RunLoadTestAsync(
                "/api/dashboard/stats",
                concurrentRequests: 20,
                requestsPerClient: 1
            );
            results.Add(result);
            
            // Small delay between rounds
            await Task.Delay(500);
        }

        // Assert - Performance should remain consistent
        var averageResponseTimes = results.Select(r => r.AverageResponseTime.TotalMilliseconds).ToList();
        var maxAvg = averageResponseTimes.Max();
        var minAvg = averageResponseTimes.Min();
        
        // Variation should not be too large (indicating degradation)
        (maxAvg - minAvg).Should().BeLessThan(500); // Less than 500ms variation
        
        _output.WriteLine($"Sustained Load: Average response times - Min: {minAvg:F2}ms, Max: {maxAvg:F2}ms");
    }

    public void Dispose()
    {
        _loadTestRunner.Dispose();
    }
}
