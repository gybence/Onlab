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
using OnlabNews.Services.SettingsServices;
using OnlabNews.Models;
using OnlabNews.Extensions;

namespace OnlabNews.Services.DataSourceServices
{
	public class ArticleDataSource : IArticleDataSourceService
	{
		#region properties

		private ISettingsService _settingsService;

		ObservableCollection<MutableGrouping<char, ArticleItem>> _groupedArticles = new ObservableCollection<MutableGrouping<char, ArticleItem>>();
		public ObservableCollection<MutableGrouping<char, ArticleItem>> GroupedArticles { get { return _groupedArticles; } set { _groupedArticles = value; } }

		CoreDispatcher dispatcher;

		#endregion

		public ArticleDataSource(ISettingsService settingsService)
		{
			_settingsService = settingsService;
			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

			//Settings.Instance.PropertyChanged += QueryArticles;
			Task.Run(QueryArticles);

			_settingsService.OnUpdateStatus += QueryArticles;
		}

		public async Task QueryArticles()
		{
			if (GroupedArticles.Count != 0)
				GroupedArticles.Clear();

			using (var db = new AppDbContext())
			{
				SyndicationClient client = new SyndicationClient();
				client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
				foreach (Subscription f in db.Subscriptions.Where(u => u.UserID == _settingsService.ActiveUser.ID).Include(r => r.RssFeed).ToList())
				{
					Uri uri = new Uri(f.RssFeed.Uri);
					SyndicationFeed feed = await client.RetrieveFeedAsync(uri);

					List<ArticleItem> list = new List<ArticleItem>();

					foreach (SyndicationItem item in feed.Items)
					{
						string itemTitle = item.Title == null ? "No title" : item.Title.Text;
						string itemLink = item.Links == null ? "No link" : item.Links.FirstOrDefault().Uri.ToString();
						
						list.Add(new ArticleItem { Title = itemTitle, Uri = itemLink, Key = itemTitle.First() });
					}

					//list.Sort();
					await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
					{
						MakeGroups(list);
					});
				}
			}
		}

		private void MakeGroups(List<ArticleItem> articles)
		{
			var groups = from c in articles
						 group c by c.Key;

			foreach (var g in groups)
			{
				var existing = GroupedArticles.FirstOrDefault(e => e.Key == g.Key);
				if (existing != null)
				{
					existing.AddToGrouping(g);
				}
				else
				{
					var mutableGroup = new MutableGrouping<char, ArticleItem>(g);
					//GroupedArticles.Add(mutableGroup);
					GroupedArticles.BinaryInsert(mutableGroup);
				}
			}
		}

	}
}
