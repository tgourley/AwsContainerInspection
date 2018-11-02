using Newtonsoft.Json;

namespace AwsContainerInspection
{
    public class AwsContainerLabels
    {
        [JsonProperty(PropertyName = "com.amazonaws.ecs.cluster")]
        public string com_amazonaws_ecs_cluster { get; set; }

        [JsonProperty(PropertyName = "com.amazonaws.ecs.container-name")]
        public string com_amazonaws_ecs_container_name { get; set; }

        [JsonProperty(PropertyName = "com.amazonaws.ecs.task-arn")]
        public string com_amazonaws_ecs_task_arn { get; set; }

        [JsonProperty(PropertyName = "com.amazonaws.ecs.task-definition-family")]
        public string com_amazonaws_ecs_task_definition_family { get; set; }

        [JsonProperty(PropertyName = "com.amazonaws.ecs.task-definition-version")]
        public string com_amazonaws_ecs_task_definition_version { get; set; }
    }
}