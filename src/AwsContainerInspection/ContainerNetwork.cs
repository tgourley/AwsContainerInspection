using System.Collections.Generic;
using System.Net;

namespace AwsContainerInspection
{
    public class ContainerNetwork
    {
        public string NetworkMode { get; set; }
        public IEnumerable<IPAddress> IPv4Addresses { get; set; }
    }
}
