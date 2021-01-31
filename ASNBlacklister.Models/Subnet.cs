using System.Net;

namespace ASNDenier.Models
{
	public record Subnet(IPAddress? IPAddress, byte? Mask)
	{
		public Subnet() : this(default, default) { }

		public override string ToString()
		{
			if (IPAddress is null) return string.Empty;
			if (Mask is null) return IPAddress.ToString();
			return $"{IPAddress}/{Mask}";
		}
	}
}
