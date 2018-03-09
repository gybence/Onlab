using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Models
{
	public class RangeObservableCollection<T> : ObservableCollection<T>
	{
		private bool _suppressNotification = false;
		protected bool SuppressNotification { get => _suppressNotification; set => _suppressNotification = value; }

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (!_suppressNotification)	
					base.OnCollectionChanged(e);
		}

		public void AddRange(IEnumerable<T> list)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			if (!list.Any()) return;

			var ev = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, base.Count);

			SuppressNotification = true;
			foreach (T item in list)
				Add(item);
			SuppressNotification = false;
			OnCollectionChanged(ev);
		}
	}
}
