namespace AwsContainerInspection
{
    public class ContainerPortMapping
    {
        public int ContainerPort { get; set; }
        public int HostPort { get; set; }
        public string BindIp { get; set; }
        public string Protocol { get; set; }
    }
}
