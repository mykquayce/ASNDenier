using System.Net;

namespace ASNBlacklister.Models
{
	public record Subnet(IPAddress? IPAddress, byte? Mask)
	{
		public Subnet() : this(default, default) { }
	}
}
