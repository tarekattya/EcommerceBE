namespace Ecommerce.Tests.LoadTesting;

/// <summary>
/// Load testing utility for testing API endpoints under various load conditions
/// </summary>
public class LoadTestRunner
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public LoadTestRunner(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    /// <summary>
    /// Runs load test with specified number of concurrent requests
    /// </summary>
    public async Task<LoadTestResult> RunLoadTestAsync(
        string endpoint,
        int concurrentRequests,
        int requestsPerClient = 1,
        HttpMethod? method = null,
        object? requestBody = null)
    {
        method ??= HttpMethod.Get;
        
        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task<LoadTestRequestResult>>();
        var requestNumber = 0;

        for (int i = 0; i < concurrentRequests; i++)
        {
            for (int j = 0; j < requestsPerClient; j++)
            {
                var reqNum = Interlocked.Increment(ref requestNumber);
                tasks.Add(ExecuteRequestAsync(endpoint, method, requestBody, reqNum));
            }
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        return new LoadTestResult
        {
            TotalRequests = results.Length,
            ConcurrentRequests = concurrentRequests,
            TotalDuration = stopwatch.Elapsed,
            SuccessfulRequests = results.Count(r => r.IsSuccess),
            FailedRequests = results.Count(r => !r.IsSuccess),
            AverageResponseTime = TimeSpan.FromMilliseconds(
                results.Average(r => r.ResponseTime.TotalMilliseconds)),
            MinResponseTime = results.Min(r => r.ResponseTime),
            MaxResponseTime = results.Max(r => r.ResponseTime),
            RequestsPerSecond = results.Length / stopwatch.Elapsed.TotalSeconds,
            Results = results.ToList()
        };
    }

    private async Task<LoadTestRequestResult> ExecuteRequestAsync(
        string endpoint,
        HttpMethod method,
        object? requestBody,
        int requestNumber)
    {
        var requestStopwatch = Stopwatch.StartNew();
        var result = new LoadTestRequestResult
        {
            RequestNumber = requestNumber,
            Endpoint = endpoint,
            Method = method.ToString()
        };

        try
        {
            HttpRequestMessage? request = null;
            
            if (method == HttpMethod.Get)
            {
                request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            }
            else if (method == HttpMethod.Post && requestBody != null)
            {
                request = new HttpRequestMessage(HttpMethod.Post, endpoint)
                {
                    Content = JsonContent.Create(requestBody)
                };
            }

            if (request == null)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "Invalid request configuration";
                return result;
            }

            var response = await _httpClient.SendAsync(request);
            requestStopwatch.Stop();

            result.ResponseTime = requestStopwatch.Elapsed;
            result.StatusCode = (int)response.StatusCode;
            result.IsSuccess = response.IsSuccessStatusCode;
            result.ResponseSize = (await response.Content.ReadAsStringAsync()).Length;
        }
        catch (Exception ex)
        {
            requestStopwatch.Stop();
            result.ResponseTime = requestStopwatch.Elapsed;
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}

public class LoadTestResult
{
    public int TotalRequests { get; set; }
    public int ConcurrentRequests { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public TimeSpan MinResponseTime { get; set; }
    public TimeSpan MaxResponseTime { get; set; }
    public double RequestsPerSecond { get; set; }
    public List<LoadTestRequestResult> Results { get; set; } = new();

    public double SuccessRate => TotalRequests > 0 ? (SuccessfulRequests / (double)TotalRequests) * 100 : 0;

    public void PrintReport()
    {
        Console.WriteLine("\n========== LOAD TEST REPORT ==========");
        Console.WriteLine($"Total Requests: {TotalRequests}");
        Console.WriteLine($"Concurrent Requests: {ConcurrentRequests}");
        Console.WriteLine($"Total Duration: {TotalDuration.TotalSeconds:F2}s");
        Console.WriteLine($"Successful: {SuccessfulRequests}");
        Console.WriteLine($"Failed: {FailedRequests}");
        Console.WriteLine($"Success Rate: {(SuccessfulRequests / (double)TotalRequests) * 100:F2}%");
        Console.WriteLine($"Average Response Time: {AverageResponseTime.TotalMilliseconds:F2}ms");
        Console.WriteLine($"Min Response Time: {MinResponseTime.TotalMilliseconds:F2}ms");
        Console.WriteLine($"Max Response Time: {MaxResponseTime.TotalMilliseconds:F2}ms");
        Console.WriteLine($"Requests Per Second: {RequestsPerSecond:F2}");
        Console.WriteLine("======================================\n");
    }
}

public class LoadTestRequestResult
{
    public int RequestNumber { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public TimeSpan ResponseTime { get; set; }
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public int ResponseSize { get; set; }
    public string? ErrorMessage { get; set; }
}
