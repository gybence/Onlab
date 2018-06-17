using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Models
{
	public class Pair<T1, T2> : INotifyPropertyChanged
	{
		private T2 _item2;
		private T1 _item1;

		public event PropertyChangedEventHandler PropertyChanged;

		public T1 Item1 { get => _item1;
			set
			{
				_item1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item1)));
			}
		}
		public T2 Item2 { get => _item2;
			set
			{
				_item2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item2)));
			}
		}

		public Pair(T1 t1, T2 t2)
		{
			Item1 = t1;
			Item2 = t2;
		}
	}
}
