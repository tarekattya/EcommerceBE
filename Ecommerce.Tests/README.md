# Ecommerce Tests

This project contains comprehensive tests for the Ecommerce application including:
- Unit Tests
- Integration Tests
- Concurrency Tests
- Load Tests

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Categories
```bash
# Unit tests only
dotnet test --filter Category=Unit

# Concurrency tests
dotnet test --filter Category=Concurrency

# Load tests
dotnet test --filter Category=Load
```

### Run with Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Test Structure

### Services Tests
- `DashboardServiceTests.cs` - Tests for dashboard service including null safety, parallel execution verification

### Concurrency Tests
- `DashboardConcurrencyTests.cs` - Tests for handling concurrent requests, race conditions, and thread safety

### Load Tests
- `DashboardLoadTests.cs` - Load testing for dashboard endpoint under various load conditions
- `LoadTestRunner.cs` - Reusable load testing utility

## Load Testing

To run load tests against a running API:

1. Start your API:
```bash
dotnet run --project Ecommerce.API
```

2. Run load tests:
```bash
dotnet test --filter Category=Load
```

Or use the standalone load test runner:
```bash
dotnet run --project Ecommerce.Tests -- LoadTesting/RunLoadTests.cs https://localhost:7021
```

## Test Coverage Goals

- Unit Test Coverage: >80%
- Integration Test Coverage: >70%
- Critical Path Coverage: 100%

## Notes

- Load tests require the API to be running
- Some tests may require database setup
- Concurrency tests verify thread safety and parallel execution