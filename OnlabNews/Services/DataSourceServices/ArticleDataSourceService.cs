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
using System.Threading;
using System.Net;

namespace OnlabNews.Services.DataSourceServices
{
	public class ArticleDataSourceService : IArticleDataSourceService
	{
		#region properties

		private ISettingsService _settingsService;

		ObservableCollection<MutableGrouping<int, ArticleItem>> _groupedArticles = new ObservableCollection<MutableGrouping<int, ArticleItem>>();
		public ObservableCollection<MutableGrouping<int, ArticleItem>> GroupedArticles { get { return _groupedArticles; } set { _groupedArticles = value; } }

		CoreDispatcher dispatcher;

		#endregion

		public ArticleDataSourceService(ISettingsService settingsService)
		{
			_settingsService = settingsService;
			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;


			Task.Run( ()=> QueryArticles(_settingsService.Cts.Token), _settingsService.Cts.Token);

			//Settings.Instance.PropertyChanged += QueryArticles;
			_settingsService.OnUpdateStatus += QueryArticles;
		}

		public async Task QueryArticles(CancellationToken ct)
		{
			//if (cts != null)
			//	cts.Cancel();
			//cts = new CancellationTokenSource();
			//var ct = cts.Token;
			

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
					if (ct.IsCancellationRequested)
					{
						//_settingsService.ActiveUser = db.Users.ToList().SingleOrDefault(u => u.Name.Equals("reddit"));
						System.Diagnostics.Debug.WriteLine("cancellation: before going through list");
						return;
					}
					List<ArticleItem> list = new List<ArticleItem>();

					foreach (SyndicationItem item in feed.Items)
					{
						try
						{
							string itemTitle = item.Title == null ? "No title" : item.Title.Text;
							string itemLink = item.Links == null ? "No link" : item.Links.FirstOrDefault().Uri.ToString();
							var published = item.PublishedDate.LocalDateTime;


							var itemElementExtensions = item.ElementExtensions.ToList();
							string itemImageUri = itemElementExtensions.FirstOrDefault(x => x.NodeName == "thumbnail") == null ? "ms-appx:///Assets/StoreLogo.png" :
													itemElementExtensions.ToList().FirstOrDefault(x => x.NodeName == "thumbnail").AttributeExtensions.ToList().FirstOrDefault(y => y.Name == "url") == null ? "ms-appx:///Assets/StoreLogo.png" :
														itemElementExtensions.ToList().FirstOrDefault(x => x.NodeName == "thumbnail").AttributeExtensions.ToList().FirstOrDefault(y => y.Name == "url").Value;

							var time = DateTime.Now - published;


							list.Add(new ArticleItem { Title = itemTitle, Uri = itemLink, ImageUri = itemImageUri, Published = published, Key = time.Hours > 24 ? 24 : time.Hours});
						}
						catch
						{
					
						}
							
					}
					await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
					{
						MakeGroups(list);
					});
					if (ct.IsCancellationRequested)
					{
						System.Diagnostics.Debug.WriteLine("cancellation: at MakeGroups()");
						return;
					}
				}
			}
		}

		private void MakeGroups(List<ArticleItem> articles)
		{
			articles.Sort();
			var groups = from c in articles
						 group c by c.Key;

			foreach (var g in groups)
			{
				var existing = GroupedArticles.FirstOrDefault(e => e.Key == g.Key);
				if (existing != null)
				{
					existing.InsertToGrouping(g);
				}
				else
				{
					var mutableGroup = new MutableGrouping<int, ArticleItem>(g);
					//GroupedArticles.Add(mutableGroup);
					GroupedArticles.BinaryInsert(mutableGroup);
				}
			}
		}

	}
}
