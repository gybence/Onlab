using Newtonsoft.Json;
using OnlabNews.Models;
using OnlabNews.Models.Scrapy;
using OnlabNews.Services.DataSourceServices;
using OnlabNews.Services.SettingsServices;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;

namespace OnlabNews.ViewModels
{
	public class ArticlePageViewModel : ViewModelBase
	{
		#region properties

		private INavigationService _navigationService;

		private ISettingsService _settingsService;
		private IScrapyService _scrapyService;
		private DataTransferManager _dataTransferManager;

		private CoreDispatcher dispatcher;

		public DelegateCommand ShareButtonCommand { get; private set; }

		private ArticleItem _article;
		public ArticleItem Article
		{
			get { return _article; }
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

		private bool _noInternet;
		public bool NoInternet
		{
			get { return _noInternet; }
			set { SetProperty(ref _noInternet, value); }
		}

		private bool _isBusy;
		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}

		#endregion

		public ArticlePageViewModel(INavigationService navigationService, ISettingsService settingsService, IScrapyService scrapyService)
		{
			_navigationService = navigationService;
			_settingsService = settingsService;
			_scrapyService = scrapyService;
			ShareButtonCommand = new DelegateCommand(ShareButtonClick);
			_useBrowser = false;
			_noInternet = false;
			_dataTransferManager = DataTransferManager.GetForCurrentView();
			_dataTransferManager.DataRequested += DataRequestedForSharing;
			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
		}

		private void DataRequestedForSharing(DataTransferManager sender, DataRequestedEventArgs args)
		{
			if (_settingsService.ActiveUser.LightweightModeEnabled)
			{
				args.Request.Data.Properties.Title = ScrapedArticle.Title;
				args.Request.Data.SetWebLink(new Uri(ScrapedArticle.Url));
			}
			else
			{
				args.Request.Data.Properties.Title = Article.Title;
				args.Request.Data.SetWebLink(new Uri(Article.Uri));
			}
			args.Request.Data.Properties.Description = "Share this article!";
		}

		private void ShareButtonClick()
		{
			if (Article != null || ScrapedArticle != null)
			{
				DataTransferManager.ShowShareUI();
			}

		}

		public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			if (NetworkInterface.GetIsNetworkAvailable())
			{
				if (_settingsService.ActiveUser.LightweightModeEnabled)
				{
					IsBusy = true;
					var s = JsonConvert.DeserializeObject<ArticleItem>(e.Parameter.ToString());
					//await dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
					//{
						ScrapedArticle = await _scrapyService.RequestArticleScrapeAsync(s);
					//});
					IsBusy = false;

					if (ScrapedArticle == null && NetworkInterface.GetIsNetworkAvailable())
					{ //azért kell itt is ellenörizni, mert lehet hogy a kérés feldolgozása között elment az internet
						UseBrowser = true;
						Article = s;
					}
					else if (ScrapedArticle == null && !NetworkInterface.GetIsNetworkAvailable())
					{ //azért kell itt is ellenörizni, mert lehet hogy a kérés feldolgozása között elment az internet
						NoInternet = true;	
					}
				}
				else
				{
					UseBrowser = true;
					Article = JsonConvert.DeserializeObject<ArticleItem>(e.Parameter.ToString());
				}
			}
			else
			{
				NoInternet = true;
			}

			base.OnNavigatedTo(e, viewModelState);
		}
	}

}
