using System.Collections.Generic;

//https://docs.aws.amazon.com/AmazonECS/latest/developerguide/container-metadata.html

namespace AwsContainerInspection
{
    public class AwsContainerMetadata
    {
        public string Cluster { get; set; }
        public string ContainerInstanceARN { get; set; }
        public string TaskARN { get; set; }
        public string ContainerID { get; set; }
        public string ContainerName { get; set; }
        public string DockerContainerName { get; set; }
        public string ImageID { get; set; }
        public string ImageName { get; set; }
        public IEnumerable<AwsContainerPortMapping> PortMappings { get; set; }
        public IEnumerable<AwsContainerNetwork> Networks { get; set; }
        public string MetadataFileStatus { get; set; }

        public string GetContainerInstanceGuid()
        {
            if (!string.IsNullOrWhiteSpace(ContainerInstanceARN))
            {
                int start = ContainerInstanceARN.LastIndexOf('/');
                if (start >= 0 && ContainerInstanceARN.Length > start + 1)
                {
                    return ContainerInstanceARN.Substring(start);
                }
            }

            return string.Empty;
        }

        public string GetTaskGuid()
        {
            if (!string.IsNullOrWhiteSpace(TaskARN))
            {
                int start = TaskARN.LastIndexOf('/');
                if (start >= 0 && TaskARN.Length > start + 1)
                {
                    return TaskARN.Substring(start);
                }
            }

            return string.Empty;
        }
    }
}
