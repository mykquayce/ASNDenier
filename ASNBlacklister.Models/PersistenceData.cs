﻿using System;
using System.Collections.Generic;
using System.Net;

namespace ASNDenier.Models
{
	public record PersistenceData(ICollection<int>? ASNNumbers, ICollection<Subnet>? Subnets)
	{
		public PersistenceData() : this(default, default) { }
	}
}
