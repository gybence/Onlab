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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
			}
		}

		string _uri;
		public string Uri
		{
			get => _uri;
			set
			{
				_uri = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Uri)));
			}
		}

		string _imageUri;
		public string ImageUri { get => _imageUri;
			set
			{
				_imageUri = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageUri)));
			}
		}
		


		int _key;
		public int Key { get => _key;
			set
			{
				//if (Char.IsPunctuation(value))
				//	_key = '&';
				//else if (Char.IsNumber(value))
				//	_key = '#';
				//else
				//	_key = Char.ToUpper(value);
				_key = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Key)));
			}
		}

		DateTime _published;
		public DateTime Published
		{
			get => _published;
			set
			{
				_published = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Published)));
			}
		}
		#endregion

		public ArticleItem()
		{

		}

		public int CompareTo(object obj)
		{
			//return string.Compare(Title, (obj as ArticleItem).Title);
			return DateTime.Compare((obj as ArticleItem).Published, Published);
		}
	}
}
