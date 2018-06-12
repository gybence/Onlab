using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Models
{
    public class BooleanWithIndex : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;

		private bool _value;
		public bool Value
		{
			get { return _value; }
			set
			{
				_value = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
			}
		}

		private int _index;
		public int Index
		{
			get { return _index; }
			set
			{
				_index = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Index)));
			}
		}
		public BooleanWithIndex(bool v, int i)
		{
			Value = v;
			Index = i;
		}

	}
}
