using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Model
{
    public class Follow
    {
		public int FollowID { get; set; }
		public int UserID { get; set; }
		public int RssItemID { get; set; }

		public User User { get; set; }
		public RssItem RssItem { get; set; }
	}
}
