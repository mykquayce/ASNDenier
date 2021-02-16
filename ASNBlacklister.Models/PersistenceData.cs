using System.Collections.Generic;

namespace ASNBlacklister.Models
{
	public record PersistenceData(ICollection<int>? ASNNumbers, ICollection<Helpers.Networking.Models.SubnetAddress>? SubnetAddresses)
	{
		public PersistenceData() : this(default, default) { }
	}
}
