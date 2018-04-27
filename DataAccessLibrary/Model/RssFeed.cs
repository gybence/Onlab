using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
	public class RssFeed : NotificationEntity
	{
		private int _id;
		private string _name;
		private string _uri;

		public int ID
		{
			get { return _id; }
			set { SetWithNotify(value, ref _id); }
		}

		public string Name
		{
			get { return _name; }
			set { SetWithNotify(value, ref _name); }
		}

		public string Uri
		{
			get { return _uri; }
			set { SetWithNotify(value, ref _uri); }
		}

		public ICollection<Subscription> Subscriptions { get; } = new ObservableHashSet<Subscription>();
	}
}
