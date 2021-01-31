using System.Net;

namespace ASNDenier.Models
{
	public record Subnet(IPAddress? IPAddress, byte? Mask)
	{
		public Subnet() : this(default, default) { }
	}
}
