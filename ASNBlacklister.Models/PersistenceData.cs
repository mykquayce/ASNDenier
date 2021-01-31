using System;
using System.Collections.Generic;

namespace ASNDenier.Models
{
	public record PersistenceData(ICollection<int>? ASNNumbers)
	{
		public PersistenceData() : this(Array.Empty<int>()) { }
	}
}
