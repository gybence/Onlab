using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlabNews.Views;
using OnlabNews.Models;
using Prism.Commands;
using Windows.System;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.DataTransfer;

namespace OnlabNews.ViewModels
{
	public class ArticlePageViewModel: ViewModelBase
	{
		#region properties

		private INavigationService _navigationService;

		DataTransferManager _dataTransferManager;

		public DelegateCommand ShareButtonCommand { get; private set; }

		private ArticleItem _article;
		public ArticleItem Article
		{
			get
			{																	//bing >_<
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

		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			Article = e.Parameter as ArticleItem;
			base.OnNavigatedTo(e, viewModelState);
		}
	}
}
