using System.Collections.Generic;

//https://docs.aws.amazon.com/AmazonECS/latest/developerguide/container-metadata.html

namespace AwsContainerInspection
{
    public class AwsContainerFile
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
            return ContainerInstanceARN.GetGuidFromEndOfArn();
        }

        public string GetTaskGuid()
        {
            return TaskARN.GetGuidFromEndOfArn();
        }
    }
}
