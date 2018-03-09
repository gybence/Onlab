using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.ViewModels
{
	public class ArticleItem : INotifyPropertyChanged, IComparable
	{
		#region properties

		public event PropertyChangedEventHandler PropertyChanged;

		string _title;
		public string Title
		{
			get => _title;
			set
			{
				_title = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_title)));
			}
		}

		string _uri;
		public string Uri
		{
			get => _uri;
			set
			{
				_uri = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_uri)));
			}
		}

		char _key;
		public char Key { get => _key;
			set
			{	
				if (Char.IsPunctuation(value))
					_key = '&';
				else if (Char.IsNumber(value))
					_key = '#';
				else
					_key = Char.ToUpper(value);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_key)));
			}
		}

		#endregion

		public ArticleItem()
		{

		}

		public int CompareTo(object obj)
		{
			return string.Compare(Title, (obj as ArticleItem).Title);
		}
	}
}
