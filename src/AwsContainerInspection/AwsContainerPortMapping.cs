namespace AwsContainerInspection
{
    public class AwsContainerPortMapping
    {
        public int ContainerPort { get; set; }
        public int HostPort { get; set; }
        public string BindIp { get; set; }
        public string Protocol { get; set; }
    }
}
