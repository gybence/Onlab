using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Models
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

		string _sourceFeedName;
		public string SourceFeedName
		{
			get { return _sourceFeedName; }
			set
			{
				_sourceFeedName = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SourceFeedName)));
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
