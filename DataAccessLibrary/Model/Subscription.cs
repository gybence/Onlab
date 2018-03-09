using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Model
{
    public class Subscription
    {
		public int SubscriptionID { get; set; }
		public int UserID { get; set; }
		public int RssFeedID { get; set; }

		public User User { get; set; }
		public RssFeed RssFeed { get; set; }
	}
}
