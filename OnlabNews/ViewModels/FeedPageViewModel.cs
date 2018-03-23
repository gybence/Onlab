using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OnlabNews.Models;
using OnlabNews.Services.DataSourceServices;
using Microsoft.Toolkit.Uwp.Services.Facebook;
using OnlabNews.Services.FacebookServices;

namespace OnlabNews.ViewModels
{
	public class FeedPageViewModel : ViewModelBase
	{

		#region properties

		private IArticleDataSourceService _articleDataSource;
		private INavigationService _navigationService;
		private IFacebookGraphService _facebookGraphService;

		public DelegateCommand<object> OnItemClickCommand { get; private set; }

		public ObservableCollection<MutableGrouping<int, ArticleItem>> GroupedArticles { get { return _articleDataSource.GroupedArticles; } }
		public RangeObservableCollection<FacebookPost> FacebookPosts { get { return _facebookGraphService.FacebookPosts; } }

		#endregion

		public FeedPageViewModel(INavigationService navigationService, IArticleDataSourceService dataSourceService, IFacebookGraphService facebookGraphService)
		{
			_articleDataSource = dataSourceService;
			_navigationService = navigationService;
			_facebookGraphService = facebookGraphService;
			OnItemClickCommand = new DelegateCommand<object>(OnClickNavigate);
			_facebookGraphService.LoadFacebookPosts();
		}

		public void OnClickNavigate(object clickedItem)
		{
			_navigationService.Navigate("Article", clickedItem);	
		}

		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(e, viewModelState);
		}
		public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
		{
			base.OnNavigatingFrom(e, viewModelState, suspending);
		}
	}
}
