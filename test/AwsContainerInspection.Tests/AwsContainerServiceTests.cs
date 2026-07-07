using AwsContainerInspection;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace AwsContainerInspection.Tests
{
    public class AwsContainerServiceTests
    {
        private const string MetadataFileEnvVar = "ECS_CONTAINER_METADATA_FILE";

        private static string FixturePath(string fileName)
        {
            return Path.Combine(AppContext.BaseDirectory, fileName);
        }

        // ----- GetMetadataFromEndpoint(string) -----

        [Fact]
        public void GetMetadataFromEndpoint_ParsesSampleTaskMetadata()
        {
            var json = File.ReadAllText(FixturePath("ecs-task-metadata-endpoint.json"));

            var metadata = AwsContainerService.GetMetadataFromEndpoint(json);

            Assert.NotNull(metadata);
            Assert.Equal("asc-adex-web", metadata.Family);
            Assert.Equal("RUNNING", metadata.KnownStatus);
            Assert.Equal("782c512c-4413-4fcc-bf24-8ae62a34adca", metadata.GetTaskGuid());
            Assert.Equal(2, metadata.Containers.Count());
        }

        [Fact]
        public void GetMetadataFromEndpoint_ParsesContainerDockerName()
        {
            var json = File.ReadAllText(FixturePath("ecs-task-metadata-endpoint.json"));

            var metadata = AwsContainerService.GetMetadataFromEndpoint(json);

            var appContainer = metadata.Containers.Single(c => c.Name == "asc-adex-web");
            Assert.Equal("ecs-asc-adex-web-31-asc-adex-web-a6afd4fcf693cef09501", appContainer.DockerName);
        }

        [Fact]
        public void GetMetadataFromEndpoint_ParsesTaskLevelLimits()
        {
            var json = File.ReadAllText(FixturePath("ecs-task-metadata-endpoint.json"));

            var metadata = AwsContainerService.GetMetadataFromEndpoint(json);

            Assert.NotNull(metadata.Limits);
            Assert.Equal(1m, metadata.Limits.CPU);
            Assert.Equal(4096m, metadata.Limits.Memory);
        }

        [Fact]
        public void GetMetadataFromEndpoint_ParsesDecimalLimits()
        {
            // Regression: Limits.CPU/Memory must be decimal so fractional CPU values
            // (e.g. a 0.25 vCPU reservation) do not fail to deserialize. (commit dc44b3b)
            var json = "{ \"TaskARN\": \"arn:aws:ecs:task/abc\", \"Limits\": { \"CPU\": 0.25, \"Memory\": 512.5 } }";

            var metadata = AwsContainerService.GetMetadataFromEndpoint(json);

            Assert.NotNull(metadata);
            Assert.Equal(0.25m, metadata.Limits.CPU);
            Assert.Equal(512.5m, metadata.Limits.Memory);
        }

        [Theory]
        [InlineData("not json at all")]
        [InlineData("{ \"unterminated\": ")]
        [InlineData("")]
        [InlineData(null)]
        public void GetMetadataFromEndpoint_ReturnsNull_ForInvalidJson(string input)
        {
            Assert.Null(AwsContainerService.GetMetadataFromEndpoint(input));
        }

        // ----- GetMetadataFromFile() -----

        [Fact]
        public void GetMetadataFromFile_ParsesSampleContainerMetadata()
        {
            using (new MetadataFileEnvScope(FixturePath("ecs-container-metadata.json")))
            {
                var metadata = AwsContainerService.GetMetadataFromFile();

                Assert.NotNull(metadata);
                Assert.Equal("default", metadata.Cluster);
                Assert.Equal("metadata", metadata.ContainerName);
                Assert.Equal("2b88376d-aba3-4950-9ddf-bcb0f388a40c", metadata.GetTaskGuid());
                Assert.Equal("1f73d099-b914-411c-a9ff-81633b7741dd", metadata.GetContainerInstanceGuid());
                Assert.Single(metadata.PortMappings);
                Assert.Single(metadata.Networks);
            }
        }

        [Fact]
        public void GetMetadataFromFile_ReturnsNull_WhenEnvVarNotSet()
        {
            using (new MetadataFileEnvScope(null))
            {
                Assert.Null(AwsContainerService.GetMetadataFromFile());
            }
        }

        [Fact]
        public void GetMetadataFromFile_ReturnsNull_WhenFileMissing()
        {
            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

            using (new MetadataFileEnvScope(missing))
            {
                Assert.Null(AwsContainerService.GetMetadataFromFile());
            }
        }

        [Fact]
        public void GetMetadataFromFile_ReturnsNull_AndDoesNotThrow_ForMalformedJson()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
            File.WriteAllText(path, "{ this is not : valid json ]");

            try
            {
                using (new MetadataFileEnvScope(path))
                {
                    Assert.Null(AwsContainerService.GetMetadataFromFile());
                }
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Sets ECS_CONTAINER_METADATA_FILE for the duration of a test and restores
        /// the previous value on dispose, so file-based tests don't leak state.
        /// </summary>
        private sealed class MetadataFileEnvScope : IDisposable
        {
            private readonly string _previous;

            public MetadataFileEnvScope(string value)
            {
                _previous = Environment.GetEnvironmentVariable(MetadataFileEnvVar);
                Environment.SetEnvironmentVariable(MetadataFileEnvVar, value);
            }

            public void Dispose()
            {
                Environment.SetEnvironmentVariable(MetadataFileEnvVar, _previous);
            }
        }
    }
}
