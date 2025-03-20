using VIAEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.OperationResultTest
{
    public class OperationResultUnitTests
    {
        [Fact]
        public void OperationResult_Succeeds_WhenCreatedWithValue()
        {
            var result = OperationResult<string>.Success("Hello, World!");

            Assert.True(result.IsSuccess);
            Assert.Equal("Hello, World!", result.Value);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void OperationResult_Succeeds_WhenCreatedWithoutValue()
        {
            var result = OperationResult<Unit>.Success();

            Assert.True(result.IsSuccess);
            Assert.Equal(Unit.Value, result.Value);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void OperationResult_Fails_WhenCreatedWithSingleError()
        {
            var result = OperationResult<string>.Failure("404", "Not Found");

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Single(result.Errors);
            Assert.Equal("404", result.Errors[0].Code);
            Assert.Equal("Not Found", result.Errors[0].Message);
        }

        [Fact]
        public void OperationResult_Fails_WhenCreatedWithMultipleErrors()
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

        [Fact]
        public void OperationResult_Succeeds_WhenImplicitlyConvertedFromValue()
        {
            OperationResult<string> result = "Hello, World!";

            Assert.True(result.IsSuccess);
            Assert.Equal("Hello, World!", result.Value);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void OperationResult_Fails_WhenImplicitlyConvertedFromError()
        {
            OperationResult<string> result = new Error("404", "Not Found");

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Single(result.Errors);
            Assert.Equal("404", result.Errors[0].Code);
        }

        [Fact]
        public void OperationResult_Fails_WhenImplicitlyConvertedFromErrorList()
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

        [Fact]
        public void OperationResult_Succeeds_WhenCombiningSuccessfulResults()
        {
            var result1 = OperationResult<string>.Success("Result 1");
            var result2 = OperationResult<string>.Success("Result 2");

            var combinedResult = OperationResult<string>.Combine(result1, result2);

            Assert.True(combinedResult.IsSuccess);
            Assert.Equal("Result 2", combinedResult.Value);
            Assert.Empty(combinedResult.Errors);
        }

        [Fact]
        public void OperationResult_Fails_WhenCombiningFailedResults()
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

        [Fact]
        public void OperationResult_Fails_WhenCombiningMixedResults()
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
