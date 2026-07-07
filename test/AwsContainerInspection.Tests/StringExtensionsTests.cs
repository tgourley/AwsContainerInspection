using AwsContainerInspection;
using Xunit;

namespace AwsContainerInspection.Tests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void GetGuidFromEndOfArn_ReturnsGuid_ForTaskArn()
        {
            var arn = "arn:aws:ecs:us-east-2:689197386106:task/782c512c-4413-4fcc-bf24-8ae62a34adca";

            Assert.Equal("782c512c-4413-4fcc-bf24-8ae62a34adca", arn.GetGuidFromEndOfArn());
        }

        [Fact]
        public void GetGuidFromEndOfArn_ReturnsSegmentAfterLastSlash()
        {
            Assert.Equal("last", "a/b/c/last".GetGuidFromEndOfArn());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetGuidFromEndOfArn_ReturnsEmpty_ForNullOrWhitespace(string input)
        {
            Assert.Equal(string.Empty, input.GetGuidFromEndOfArn());
        }

        [Fact]
        public void GetGuidFromEndOfArn_ReturnsEmpty_WhenNoSlashPresent()
        {
            Assert.Equal(string.Empty, "no-slash-here".GetGuidFromEndOfArn());
        }

        [Fact]
        public void GetGuidFromEndOfArn_ReturnsEmpty_WhenSlashIsLastCharacter()
        {
            Assert.Equal(string.Empty, "arn:aws:ecs:task/".GetGuidFromEndOfArn());
        }
    }
}
