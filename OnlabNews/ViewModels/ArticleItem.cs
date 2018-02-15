using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.ViewModels
{
	class ArticleItem : ViewModelBase
	{
		#region properties
		string _title;
		public string Title { get => _title; set { SetProperty(ref _title, value); } }

		string _uri;
		public string Uri { get => _uri; set { SetProperty(ref _uri, value); } }
		#endregion










		public ArticleItem(string title, string uri)
		{
			Title = title;
			Uri = uri;
		}
		public ArticleItem(string uri)
		{
			Title = "No Title";
			Uri = uri;
		}

	}
}
