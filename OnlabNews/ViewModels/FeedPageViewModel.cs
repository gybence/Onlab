using Newtonsoft.Json;
using OnlabNews.Models;
using OnlabNews.Services.DataSourceServices;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System.Collections.Generic;
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

		private bool _isBusy;
		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}
		#endregion

		public FeedPageViewModel(INavigationService navigationService, IArticleDataSourceService dataSourceService)
		{
			_articleDataSource = dataSourceService;
			_navigationService = navigationService;
			OnItemClickCommand = new DelegateCommand<object>(OnClickNavigate);
			RefreshButtonCommand = new DelegateCommand(OnRefreshButtonClick);
			IsBusy = true;
			Task.Run(() => _articleDataSource.CreateArticlesAsync());
			IsBusy = false;
		}

		private async void OnRefreshButtonClick()
		{
			IsBusy = true;
			await _articleDataSource.CreateArticlesAsync();
			IsBusy = false;
		}

		private void OnClickNavigate(object clickedItem)
		{
			string s = JsonConvert.SerializeObject(clickedItem);
			_navigationService.Navigate("Article", s);
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
