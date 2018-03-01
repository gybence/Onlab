using DataAccessLibrary;
using DataAccessLibrary.Model;
using Microsoft.EntityFrameworkCore;
using OnlabNews.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.Web.Syndication;

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

		ObservableCollection<ArticleItem> _articleCollection = new ObservableCollection<ArticleItem>();
		public ObservableCollection<ArticleItem> ArticleCollection { get { return _articleCollection; } set { _articleCollection = value; } }

		CoreDispatcher dispatcher;

		#endregion

		private ItemDataSource()
		{
			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

		}

		public async Task QueryArticles()
		{
			if (ArticleCollection.Count != 0)
				ArticleCollection.Clear();
			try
			{
				using (var db = new AppDbContext())
				{ 				
					SyndicationClient client = new SyndicationClient();
					client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
					foreach (Follow f in db.Follows.Where(u => u.UserID == Settings.Instance.ActiveUser.ID).Include(r => r.RssItem).ToList())
					{
						Uri uri = new Uri(f.RssItem.Uri);
						SyndicationFeed feed = await client.RetrieveFeedAsync(uri);

						foreach (SyndicationItem item in feed.Items)
							await DisplayCurrentItemAsync(item);
					}
				}
			}
			catch
			{

			}
		}
		private async Task DisplayCurrentItemAsync(SyndicationItem item)
		{

			string itemTitle = item.Title == null ? "No title" : item.Title.Text;
			string itemLink = item.Links == null ? "No link" : item.Links.FirstOrDefault().NodeValue;

			await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
			{
				ArticleCollection.Add(new ArticleItem { Title = itemTitle, Uri = itemLink });
			});

		}
	}
}
