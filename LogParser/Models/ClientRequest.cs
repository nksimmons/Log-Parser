using System.Net;

namespace LogParser.Models
{
    public class ClientRequest
    {
        public IPAddress IpAddress { get; set; }
        public string RequestType { get; set; }
        public int PortNumber { get; set; }
    }
}
