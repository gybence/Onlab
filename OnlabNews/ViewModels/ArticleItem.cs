using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.ViewModels
{
	public class ArticleItem : ViewModelBase
	{
		#region properties

		string _title;
		public string Title { get => _title; set { SetProperty(ref _title, value); } }

		string _uri;
		public string Uri { get => _uri; set { SetProperty(ref _uri, value); } }

		#endregion

		public ArticleItem()
		{

		}

	}
}
