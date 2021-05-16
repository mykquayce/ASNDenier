using System.Collections.Generic;

namespace ASNDenier.Models
{
	public record PersistenceData(ICollection<int>? ASNNumbers, ICollection<Helpers.Networking.Models.AddressPrefix>? Prefixes)
	{
		public PersistenceData() : this(default, default) { }
	}
}
