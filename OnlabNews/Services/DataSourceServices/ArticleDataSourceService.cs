using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.Web.Syndication;
using OnlabNews.Services.SettingsServices;
using OnlabNews.Models;
using OnlabNews.Extensions;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using RSSDownloader;

namespace OnlabNews.Services.DataSourceServices
{
	public class ArticleDataSourceService : IArticleDataSourceService
	{
		#region properties

		private ISettingsService _settingsService;
		private RangeObservableCollection<MutableGrouping<int, ArticleItem>> _groupedArticles = new RangeObservableCollection<MutableGrouping<int, ArticleItem>>();
		public RangeObservableCollection<MutableGrouping<int, ArticleItem>> GroupedArticles { get { return _groupedArticles; } set { _groupedArticles = value; } }

		private CoreDispatcher dispatcher;
		private BackgroundTaskRegistration _registration;


		#endregion

		public ArticleDataSourceService(ISettingsService settingsService)
		{
			_settingsService = settingsService;
			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

			RegisterBackgroundTaskAsync("TimeTriggeredTileUpdaterBackgroundTask", "Tasks.TileUpdaterBackgroundTask", new TimeTrigger(15, false));
			//RegisterBackgroundTask("ApplicationTriggeredTileUpdaterBackgroundTask", "Tasks.TileUpdaterBackgroundTask", new ApplicationTrigger());
			//Task.Run(() => CreateArticlesAsync());

		}

		public async Task CreateArticlesAsync()
		{
			try
			{
				if (GroupedArticles.Count != 0)
					GroupedArticles.Clear();

				RssFeedDownloader rfg = new RssFeedDownloader();
				var results = await rfg.DownloadFeedsAsync(_settingsService.ActiveUser);

				//results.count != 0
				List<ArticleItem> list = new List<ArticleItem>();
				foreach (SyndicationFeed feed in results)
				{
					foreach (SyndicationItem item in feed.Items)
					{
						//ct.ThrowIfCancellationRequested();

						string itemTitle = item.Title == null ? "No title" : item.Title.Text;
						string itemLink = item.Links == null ? "No link" : item.Links.FirstOrDefault().Uri.ToString();

						DateTime published;

						if(item.PublishedDate.Year != 1601)
							published = item.PublishedDate.LocalDateTime;
						else //persze nyilvan mi van ha a lastupdated is 1601 mert azt se toltottek ki rendesen... 
							published = item.LastUpdatedTime.LocalDateTime;

						string imageUri = "";
						foreach (SyndicationNode element in item.ElementExtensions)
						{
							imageUri = SearchElementForURIs(element);
							if (!String.IsNullOrEmpty(imageUri))
								break;
						}

						#region non-recursive
						if (String.IsNullOrEmpty(imageUri))
						{
							foreach (var link in item.Links) //index
							{
								string value = link.Uri.OriginalString;
								if ((value.StartsWith("http://") || value.StartsWith("https://")) && ((value.EndsWith(".jpg") || value.EndsWith(".png") || value.EndsWith(".gif"))))
								{
									imageUri = value;
									break;
								}
							}
						}
						///444 egy rakas fos lol, de item.Summary.Text-ben benne van vegulis
						///reddit: item.Content.Text-ben ... :/
						#endregion

						var time = DateTime.Now - published;
						if (time.TotalDays < 1)
						{
							list.Add(new ArticleItem
							{
								Title = itemTitle,
								Uri = itemLink,
								ImageUri = String.IsNullOrEmpty(imageUri) ? "ms-appx:///Assets/StoreLogoInverted.png" : imageUri,
								//ImageUri = itemImageUri,
								SourceFeedName = feed.Title.Text,
								Published = published,
								Key = time.Hours
							});
						}

					}
				}
				if (list.Count != 0)
				{
					await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
					{
						MakeGroups(list);
					});

					//ct.ThrowIfCancellationRequested();
					PassTileDataToBackgroundTask();
				}
			}
			catch (OperationCanceledException)
			{

			}
		}

		private string SearchElementForURIs(SyndicationNode element)
		{
			string uri = null;
			if (element.AttributeExtensions.Count != 0) //bbc
			{
				foreach (var attribute in element.AttributeExtensions)
				{
					string value = attribute.Value;
					if ((value.StartsWith("http://") || value.StartsWith("https://")) && ((value.EndsWith(".jpg") || value.EndsWith(".png") || value.EndsWith(".gif"))))
					{
						uri = value; // erdemes lehet az 1. talalat utane breakelni
						break;
					}
				}
				if(String.IsNullOrEmpty(uri))
				{
					if (element.ElementExtensions.Count != 0)
						foreach (var elem in element.ElementExtensions)
						{
							uri = SearchElementForURIs(element);
							if (!String.IsNullOrEmpty(uri))
								break;
						}
				}
			}
			else
			{
				if (element.ElementExtensions.Count != 0)
					foreach (SyndicationNode elem in element.ElementExtensions)
					{
						uri = SearchElementForURIs(elem);
						if (!String.IsNullOrEmpty(uri))
							break;
					}
			}

			return uri;
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


		#region background task

		private async void RegisterBackgroundTaskAsync(string taskName, string taskEntryPoint, IBackgroundTrigger trigger)
		{
			//BackgroundExecutionManager.RemoveAccess();
			var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
			if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
				backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
			{
				bool registered = false;
				foreach (var task in BackgroundTaskRegistration.AllTasks)
				{
					if (task.Value.Name == taskName)
					{
						//task.Value.Unregister(true);
						registered = true;
					}
				}
				if (registered == false)
				{
					BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
					taskBuilder.Name = taskName;
					taskBuilder.TaskEntryPoint = taskEntryPoint;
					taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
					taskBuilder.SetTrigger(trigger);
					_registration = taskBuilder.Register();
				}
			}
		}

		private void PassTileDataToBackgroundTask()
		{
			if (GroupedArticles.Count != 0)
			{
				var firstArticle = GroupedArticles[0][0];
				var localSettings = ApplicationData.Current.LocalSettings;
				localSettings.Values["source"] = firstArticle.SourceFeedName;
				localSettings.Values["title"] = firstArticle.Title;
				localSettings.Values["pubDate"] = firstArticle.Published.ToString();
			}
		}

		#endregion
	}
}
