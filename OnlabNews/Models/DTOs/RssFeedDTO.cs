using DataAccessLibrary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Models.DTOs
{
	public class RssFeedDTO : INotifyPropertyChanged
	{
		#region properties 

		public event PropertyChangedEventHandler PropertyChanged;

		private RssFeed _rssFeed;
		public RssFeed RssFeed
		{
			get => _rssFeed;
			set
			{
				_rssFeed = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RssFeed)));
			}
		}

		private bool _enabled;
		public bool Enabled	
		{
			get => _enabled;
			set
			{
				_enabled = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Enabled)));
			}
		}

		private bool _isReadOnly;
		public bool IsReadOnly
		{
			get => _isReadOnly;
			set
			{
				_isReadOnly = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsReadOnly)));
			}
		}

		private int _index;
		public int Index
		{
			get => _index;
			set
			{
				_index = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Index)));
			}
		}

		#endregion

		public RssFeedDTO(RssFeed rssFeed, bool enabled, int index)
		{
			_rssFeed = rssFeed;
			_enabled = enabled;
			_index = index;
			_isReadOnly = true;
		}

	}
}
