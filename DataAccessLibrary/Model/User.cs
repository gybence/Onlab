﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
	public class User
	{
		public int UserID { get; set; }
		public string UserName { get; set; }

		public ICollection<RssItem> RssItems { get; set; }
	}
}