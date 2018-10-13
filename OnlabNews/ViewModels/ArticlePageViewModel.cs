using OnlabNews.Models;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;

namespace OnlabNews.ViewModels
{
	public class ArticlePageViewModel : ViewModelBase
	{
		#region properties

		private INavigationService _navigationService;

		DataTransferManager _dataTransferManager;

		private CoreDispatcher dispatcher;

		public DelegateCommand ShareButtonCommand { get; private set; }

		private ArticleItem _article;
		public ArticleItem Article
		{
			get
			{                                                                   //bing >_<
				return _article ?? new ArticleItem { Title = "No Title", Uri = "https://bing.com" };
			}
			set { SetProperty(ref _article, value); }
		}

		#endregion

		public ArticlePageViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService;
			ShareButtonCommand = new DelegateCommand(ShareButtonClick);


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
			DataTransferManager.ShowShareUI();
		}

		private async Task RequestArticleScrapeAsync(String toScrape)
		{
			using (HttpClient httpClient = new HttpClient())
			{
				httpClient.BaseAddress = new Uri(@"http://localhost:8080");
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));

				string endpoint = @"";

				try
				{
					//JsonConvert.SerializeObject(yourPocoHere), Encoding.UTF8, "application/json"
					HttpContent content = new StringContent("{\"url\":\""+ toScrape + "\"}");
					HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

					if (response.IsSuccessStatusCode)
					{
						string jsonResponse = await response.Content.ReadAsStringAsync();
						//do something with json response here
					}
				}
				catch (Exception)
				{
					//Could not connect to server
					//Use more specific exception handling, this is just an example
				}
			}
		}


		public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			Article = e.Parameter as ArticleItem;
			base.OnNavigatedTo(e, viewModelState);
			await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
			{
				 RequestArticleScrapeAsync("file:///C:/Users/Bence/Desktop/quotes.html");
			});
			bool asd = false;
		}
	}
}
