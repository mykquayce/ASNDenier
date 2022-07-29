namespace ASNDenier.Models;

public record PersistenceData(IReadOnlyDictionary<string, int[]>? ASNNumbers, ICollection<Helpers.Networking.Models.AddressPrefix>? Prefixes)
{
	public PersistenceData() : this(default, default) { }
}
