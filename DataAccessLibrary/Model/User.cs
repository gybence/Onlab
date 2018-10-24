using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;

namespace DataAccessLibrary.Model
{
	public class User : NotificationEntity
	{
		private int _id;
		private string _name;
		private bool _lastLoggedIn;
		private bool _lightweightModeEnabled;

		public int ID { get => _id; set { SetWithNotify(value, ref _id); } }
		public string Name { get => _name; set { SetWithNotify(value, ref _name); } }
		public bool LastLoggedIn { get => _lastLoggedIn; set { SetWithNotify(value, ref _lastLoggedIn); } }
		public bool LightweightModeEnabled { get => _lightweightModeEnabled; set { SetWithNotify(value, ref _lightweightModeEnabled); } }

		public ICollection<Subscription> Subscriptions { get; set; } = new ObservableHashSet<Subscription>();
	}
}
