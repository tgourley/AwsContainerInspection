using System.Collections.Generic;
using System.Net;

namespace AwsContainerInspection
{
    public class AwsContainerNetwork
    {
        public string NetworkMode { get; set; }
        public IEnumerable<string> IPv4Addresses { get; set; }
    }
}
