using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Model
{
	public class Subscription : NotificationEntity
	{
		private int _subscriptionID;
		private int _userID;
		private int _rssFeedID;
		private User _user;
		private RssFeed _rssFeed;

		public int SubscriptionID { get => _subscriptionID; set { SetWithNotify(value, ref _subscriptionID); } }
		public int UserID { get => _userID; set { SetWithNotify(value, ref _userID); } }
		public int RssFeedID { get => _rssFeedID; set { SetWithNotify(value, ref _rssFeedID); } }

		public User User { get => _user; set { SetWithNotify(value, ref _user); } }
		public RssFeed RssFeed { get => _rssFeed; set { SetWithNotify(value, ref _rssFeed); } }
	}
}
