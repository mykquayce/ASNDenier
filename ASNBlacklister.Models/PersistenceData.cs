using System.Collections.Generic;

namespace ASNDenier.Models
{
	public record PersistenceData(ICollection<int>? ASNNumbers, ICollection<Subnet>? SubnetAddresses)
	{
		public PersistenceData() : this(default, default) { }
	}
}
