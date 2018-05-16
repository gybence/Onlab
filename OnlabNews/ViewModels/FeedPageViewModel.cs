using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OnlabNews.Models;
using OnlabNews.Services.DataSourceServices;
using OnlabNews.Services.FacebookServices;
using System;
using System.Threading.Tasks;

namespace OnlabNews.ViewModels
{
	public class FeedPageViewModel : ViewModelBase
	{

		#region properties

		private IArticleDataSourceService _articleDataSource;
		private INavigationService _navigationService;

		public DelegateCommand<object> OnItemClickCommand { get; private set; }
		public DelegateCommand RefreshButtonCommand { get; private set; }

		public RangeObservableCollection<MutableGrouping<int, ArticleItem>> GroupedArticles { get { return _articleDataSource.GroupedArticles; } }
		
		#endregion

		public FeedPageViewModel(INavigationService navigationService, IArticleDataSourceService dataSourceService)
		{
			_articleDataSource = dataSourceService;
			_navigationService = navigationService;
			OnItemClickCommand = new DelegateCommand<object>(OnClickNavigate);
			RefreshButtonCommand = new DelegateCommand(OnRefreshButtonClick);
			Task.Run(() => _articleDataSource.CreateArticlesAsync());
		}

		private async void OnRefreshButtonClick()
		{
			await _articleDataSource.CreateArticlesAsync();
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
