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
using OnlabNews.Helpers;
using System.Threading;
using System.Net;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using System.ComponentModel;
using System.ServiceModel.Dispatcher;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace OnlabNews.Services.DataSourceServices
{
	public class ArticleDataSourceService : IArticleDataSourceService
	{
		#region properties

		private ISettingsService _settingsService;

		RangeObservableCollection<MutableGrouping<int, ArticleItem>> _groupedArticles = new RangeObservableCollection<MutableGrouping<int, ArticleItem>>();
		public RangeObservableCollection<MutableGrouping<int, ArticleItem>> GroupedArticles { get { return _groupedArticles; } set { _groupedArticles = value; } }

		CoreDispatcher dispatcher;

		ApplicationTrigger trigger = null;

		#endregion

		public ArticleDataSourceService(ISettingsService settingsService)
		{
			_settingsService = settingsService;
			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
			TileUpdateManager.CreateTileUpdaterForApplication().Clear();

			Task.Run(() => QueryArticles(_settingsService.Cts.Token), _settingsService.Cts.Token);

			_settingsService.OnUpdateStatus += QueryArticles;

			trigger = new ApplicationTrigger();
			var task = RegisterBackgroundTask("Tasks.TileUpdaterBackgroundTask",
											  "Tile Updater Background Task",
											  trigger);

		}
		public BackgroundTaskRegistration RegisterBackgroundTask(String taskEntryPoint, String name, IBackgroundTrigger trigger, IBackgroundCondition condition = null)
		{
			var builder = new BackgroundTaskBuilder();

			builder.Name = name;
			builder.TaskEntryPoint = taskEntryPoint;
			builder.SetTrigger(trigger);


			BackgroundTaskRegistration task = builder.Register();
			task.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
			task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
			return task;
		}
	
		private void OnProgress(IBackgroundTaskRegistration task, BackgroundTaskProgressEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine("------			onprogress " + args.Progress);
		}

		private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
		{
			
		}
		public async Task QueryArticles(CancellationToken ct)
		{
			try
			{
				if (GroupedArticles.Count != 0)
					GroupedArticles.Clear();

				using (var db = new AppDbContext())
				{
					SyndicationClient client = new SyndicationClient();
					client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
					foreach (Subscription f in db.Subscriptions.Where(u => u.UserID == _settingsService.ActiveUser.ID).Include(r => r.RssFeed).ToList())
					{
						ct.ThrowIfCancellationRequested();
						Uri uri = new Uri(f.RssFeed.Uri);
						SyndicationFeed feed = await client.RetrieveFeedAsync(uri);
						
						List<ArticleItem> list = new List<ArticleItem>();

						foreach (SyndicationItem item in feed.Items)
						{
							ct.ThrowIfCancellationRequested();

							string itemTitle = item.Title == null ? "No title" : item.Title.Text;
							string itemLink = item.Links == null ? "No link" : item.Links.FirstOrDefault().Uri.ToString();
							var published = item.PublishedDate.LocalDateTime;

							//TODO: csak BBC-hez mukodik, lehet mindnek kulon meg kell csinalni?
							var itemElementExtensions = item.ElementExtensions.ToList();
							string itemImageUri = itemElementExtensions.FirstOrDefault(x => x.NodeName == "thumbnail") == null ? "ms-appx:///Assets/StoreLogo.png" :
													itemElementExtensions.ToList().FirstOrDefault(x => x.NodeName == "thumbnail").AttributeExtensions.ToList().FirstOrDefault(y => y.Name == "url") == null ? "ms-appx:///Assets/StoreLogo.png" :
														itemElementExtensions.ToList().FirstOrDefault(x => x.NodeName == "thumbnail").AttributeExtensions.ToList().FirstOrDefault(y => y.Name == "url").Value;

							var time = DateTime.Now - published;

							list.Add(new ArticleItem
							{
								Title = itemTitle,
								Uri = itemLink,
								ImageUri = itemImageUri,
								SourceFeedName = feed.Title.Text,
								Published = published,
								Key = time.Days > 1 ? 24 : time.Hours
							});
						}
						
						await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
						{
							MakeGroups(list);
						});
					}
				}

				ct.ThrowIfCancellationRequested();
				await StartTileUpdaterBackgroundTask();
			}
			catch (OperationCanceledException)
			{

			}
			finally
			{
				//handle when task is cancelled
				System.Diagnostics.Debug.WriteLine("-- QueryArticles cancelled! --");
			}
		}

		private async Task StartTileUpdaterBackgroundTask()
		{
			if (GroupedArticles.Count != 0)
			{
				var firstArticle = GroupedArticles[0][0];
				ValueSet set = new ValueSet
				{
					{ "SourceFeedName", firstArticle.SourceFeedName },
					{ "Title", firstArticle.Title }
				};
				var result = await trigger.RequestAsync(set);
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
