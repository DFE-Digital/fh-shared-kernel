namespace FamilyHubs.SharedKernel.UnitTests.ResultTests
{
    public class ResultTests
    {
        [Fact]
        public void SuccessMethod_BuildsSuccessfullResult()
        {
            //  Arrange / Act
            var result = Result.Success();

            //  Assert
            Assert.True(result.Succeeded);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void SuccessMethod_BuildsSuccessfullResultWithValue()
        {
            //  Arrange
            var testObect = new TestObject("Hello World");

            //  Act
            var result = Result<TestObject>.Success(testObect);

            //  Assert
            Assert.True(result.Succeeded);
            Assert.Empty(result.Errors);
            Assert.Equal("Hello World", result.Value!.SomeProp);
        }

        [Fact]
        public void SuccessMethod_BuildsFailureResult()
        {
            //  Arrange
            var errors = new List<string> { "Hello World" };

            //  Act
            var result = Result.Failure(errors);

            //  Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Hello World", result.Errors[0]);
        }

        [Fact]
        public void SuccessMethod_BuildsFailureResultForValueResult()
        {
            //  Arrange
            var errors = new List<string> { "Hello World" };

            //  Act
            var result = Result<TestObject>.Failure("testFailure", errors);

            //  Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Hello World", result.Errors[0]);
            Assert.Equal("testFailure", result.FailureType);
            Assert.Null(result.Value);
        }
    }

    public class TestObject
    {
        public string SomeProp { get; set; }

        public TestObject(string someProp)
        {
            SomeProp = someProp;
        }
    }
}
