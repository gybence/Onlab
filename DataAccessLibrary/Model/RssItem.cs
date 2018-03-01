﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
	public class RssItem
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Uri { get; set; }

		public ICollection<Follow> Follows { get; set; }
	}
}
