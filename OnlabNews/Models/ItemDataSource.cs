using OnlabNews.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;


namespace OnlabNews.Models
{
	public class ItemDataSource
	{
		#region properties

		private static ItemDataSource instance;
		public static ItemDataSource Instance
		{
			get
			{
				if (instance == null)
					instance = new ItemDataSource();
				return instance;
			}
		}


		ObservableCollection<ArticleItem> _articleCollection;
		internal ObservableCollection<ArticleItem> ArticleCollection { get { return _articleCollection; } set { _articleCollection = value; } }


		CoreDispatcher dispatcher;
		#endregion

		private ItemDataSource()
		{
			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;


			Task.Run(QueryArticles);
		}

		async Task QueryArticles()
		{
			throw new NotImplementedException();
		}
	}
}
