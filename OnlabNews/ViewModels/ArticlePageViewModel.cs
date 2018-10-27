using Newtonsoft.Json;
using OnlabNews.Models;
using OnlabNews.Models.Scrapy;
using OnlabNews.Services.SettingsServices;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;

namespace OnlabNews.ViewModels
{
	public class ArticlePageViewModel : ViewModelBase
	{
		#region properties

		private INavigationService _navigationService;

		private ISettingsService _settingsService;

		private DataTransferManager _dataTransferManager;

		private CoreDispatcher dispatcher;

		public DelegateCommand ShareButtonCommand { get; private set; }

		private ArticleItem _article;
		public ArticleItem Article
		{
			get
			{                                                                   //bing >_<
				return _article;
			}

			set { SetProperty(ref _article, value); }
		}

		private RootObject _scrapedArticle;
		public RootObject ScrapedArticle
		{
			get { return _scrapedArticle; }
			set { SetProperty(ref _scrapedArticle, value); }
		}

		private bool _useBrowser;
		public bool UseBrowser
		{
			get { return _useBrowser; }
			set { SetProperty(ref _useBrowser, value); }
		}

		#endregion

		public ArticlePageViewModel(INavigationService navigationService, ISettingsService settingsService)
		{
			_navigationService = navigationService;
			_settingsService = settingsService;
			ShareButtonCommand = new DelegateCommand(ShareButtonClick);
			_useBrowser = false;

			_dataTransferManager = DataTransferManager.GetForCurrentView();
			_dataTransferManager.DataRequested += DataRequestedForSharing;
			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
		}

		private void DataRequestedForSharing(DataTransferManager sender, DataRequestedEventArgs args)
		{
			args.Request.Data.Properties.Title = Article.Title;
			args.Request.Data.Properties.Description = "Share this article!";
			args.Request.Data.SetWebLink(new Uri(Article.Uri));

		}

		private void ShareButtonClick()
		{
			if (Article != null)
			{
				DataTransferManager.ShowShareUI();
			}
		}

		private async Task RequestArticleScrapeAsync(ArticleItem toScrape)
		{
			using (HttpClient httpClient = new HttpClient())
			{
				httpClient.BaseAddress = new Uri(@"http://localhost:5000");
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));

				string endpoint = @"/scrape";

				try
				{
					//JsonConvert.SerializeObject(yourPocoHere), Encoding.UTF8, "application/json"
					//HttpContent content = new StringContent("{\"url\":\""+ toScrape + "\"}");
					var content = new StringContent("{\"url\":\"" + "https://index.hu/sport/forma1/2018/10/13/briatore_ravilagitott_miert_bukik_vettel_iden" + "\"}", Encoding.UTF8, "application/json");
					HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

					if (response.IsSuccessStatusCode)
					{
						string jsonResponse = await response.Content.ReadAsStringAsync();
						ScrapedArticle = JsonConvert.DeserializeObject<RootObject>(jsonResponse);
						UseBrowser = false;
						//do something with json response here
						//Article = toScrape;
					}
					else
					{
						UseBrowser = true;
						Article = toScrape;
					}
				}
				catch (Exception)
				{
					UseBrowser = true;
					Article = toScrape;
					//Could not connect to server
					//Use more specific exception handling, this is just an example
				}
			}
		}

		public void GombClick()
		{
			UseBrowser = true;
		}


		public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			if (_settingsService.ActiveUser.LightweightModeEnabled)
			{
				await dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
				{
					await RequestArticleScrapeAsync(e.Parameter as ArticleItem);
				});
			}
			else
			{
				UseBrowser = true;
				Article = e.Parameter as ArticleItem;
			}
			base.OnNavigatedTo(e, viewModelState);
		}
	}
}
