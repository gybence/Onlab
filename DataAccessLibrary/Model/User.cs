using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
	public class User: NotificationEntity
	{
		private int _id;
		private string _name;
		private bool _lastLoggedIn;

		public int ID { get => _id; set { SetWithNotify(value, ref _id); } }
		public string Name { get => _name; set { SetWithNotify(value, ref _name); } }
		public bool LastLoggedIn { get => _lastLoggedIn; set { SetWithNotify(value, ref _lastLoggedIn); } }

		public ICollection<Subscription> Subscriptions { get; set; } = new ObservableHashSet<Subscription>();
	}
}
