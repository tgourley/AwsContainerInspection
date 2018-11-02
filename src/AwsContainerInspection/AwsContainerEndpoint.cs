using System;
using System.Collections.Generic;
using System.Text;

namespace AwsContainerInspection
{
    public class AwsContainerEndpoint
    {
        public string Cluster { get; set; }
        public IEnumerable<AwsContainer> Containers { get; set; }
        public string DesiredStatus { get; set; }
        public string Family { get; set; }
        public string KnownStatus { get; set; }
        public AwsContainerLimit Limits { get; set; }
        public string PullStartedAt { get; set; }
        public string PullStoppedAt { get; set; }
        public string Revision { get; set; }
        public string TaskARN { get; set; }

        public string GetTaskGuid()
        {
            return TaskARN.GetGuidFromEndOfArn();
        }
    }
}
