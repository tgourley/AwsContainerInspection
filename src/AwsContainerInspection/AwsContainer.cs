using System.Collections.Generic;

namespace AwsContainerInspection
{
    public class AwsContainer
    {
        public string CreatedAt { get; set; }
        public string DesiredStatus { get; set; }
        public string DockerId { get; set; }
        public string ContaineDockerNamerName { get; set; }
        public string Image { get; set; }
        public string ImageID { get; set; }
        public string KnownStatus { get; set; }
        public AwsContainerLabels Labels { get; set; }
        public AwsContainerLimit Limits { get; set; }
        public string Name { get; set; }
        public IEnumerable<AwsContainerNetwork> Networks { get; set; }
        public string StartedAt { get; set; }
        public string Type { get; set; }
    }
}