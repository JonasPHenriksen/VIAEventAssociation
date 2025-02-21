using VIAEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.OperationResultTest
{
    public class OperationResultUnitTests
    {
        // Test successful result with a value
        [Fact]
        public void Success_WithValue_ShouldReturnResultWithValue()
        {
            var result = OperationResult<string>.Success("Hello, World!");

            Assert.True(result.IsSuccess);
            Assert.Equal("Hello, World!", result.Value);
            Assert.Empty(result.Errors);
        }

        // Test successful result without a value (Unit)
        [Fact]
        public void Success_WithoutValue_ShouldReturnUnit()
        {
            var result = OperationResult<Unit>.Success();

            Assert.True(result.IsSuccess);
            Assert.Equal(Unit.Value, result.Value);
            Assert.Empty(result.Errors);
        }

        // Test failure with a single error
        [Fact]
        public void Failure_WithSingleError_ShouldReturnResultWithError()
        {
            var result = OperationResult<string>.Failure("404", "Not Found");

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Single(result.Errors);
            Assert.Equal("404", result.Errors[0].Code);
            Assert.Equal("Not Found", result.Errors[0].Message);
        }

        // Test failure with multiple errors
        [Fact]
        public void Failure_WithMultipleErrors_ShouldReturnResultWithErrors()
        {
            var errors = new List<Error>
            {
                new("404", "Not Found"),
                new("500", "Internal Server Error")
            };

            var result = OperationResult<string>.Failure(errors);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal(2, result.Errors.Count);
            Assert.Equal("404", result.Errors[0].Code);
            Assert.Equal("500", result.Errors[1].Code);
        }

        // Test implicit conversion from value to OperationResult
        [Fact]
        public void ImplicitConversion_FromValue_ShouldReturnSuccessResult()
        {
            OperationResult<string> result = "Hello, World!";

            Assert.True(result.IsSuccess);
            Assert.Equal("Hello, World!", result.Value);
            Assert.Empty(result.Errors);
        }

        // Test implicit conversion from Error to OperationResult
        [Fact]
        public void ImplicitConversion_FromError_ShouldReturnFailureResult()
        {
            OperationResult<string> result = new Error("404", "Not Found");

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Single(result.Errors);
            Assert.Equal("404", result.Errors[0].Code);
        }

        // Test implicit conversion from List<Error> to OperationResult
        [Fact]
        public void ImplicitConversion_FromErrorList_ShouldReturnFailureResult()
        {
            var errors = new List<Error>
            {
                new("404", "Not Found"),
                new("500", "Internal Server Error")
            };

            OperationResult<string> result = errors;

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal(2, result.Errors.Count);
        }

        // Test Combine method with successful results
        [Fact]
        public void Combine_WithSuccessfulResults_ShouldReturnSuccessResult()
        {
            var result1 = OperationResult<string>.Success("Result 1");
            var result2 = OperationResult<string>.Success("Result 2");

            var combinedResult = OperationResult<string>.Combine(result1, result2);

            Assert.True(combinedResult.IsSuccess);
            Assert.Equal("Result 2", combinedResult.Value);
            Assert.Empty(combinedResult.Errors);
        }

        // Test Combine method with failed results
        [Fact]
        public void Combine_WithFailedResults_ShouldReturnFailureResultWithAllErrors()
        {
            var result1 = OperationResult<string>.Failure("404", "Not Found");
            var result2 = OperationResult<string>.Failure("500", "Internal Server Error");

            var combinedResult = OperationResult<string>.Combine(result1, result2);

            Assert.False(combinedResult.IsSuccess);
            Assert.Null(combinedResult.Value);
            Assert.Equal(2, combinedResult.Errors.Count);
            Assert.Equal("404", combinedResult.Errors[0].Code);
            Assert.Equal("500", combinedResult.Errors[1].Code);
        }

        // Test Combine method with mixed results (success and failure)
        [Fact]
        public void Combine_WithMixedResults_ShouldReturnFailureResultWithErrors()
        {
            var result1 = OperationResult<string>.Success("Result 1");
            var result2 = OperationResult<string>.Failure("404", "Not Found");

            var combinedResult = OperationResult<string>.Combine(result1, result2);

            Assert.False(combinedResult.IsSuccess);
            Assert.Null(combinedResult.Value);
            Assert.Single(combinedResult.Errors);
            Assert.Equal("404", combinedResult.Errors[0].Code);
        }
    }
}