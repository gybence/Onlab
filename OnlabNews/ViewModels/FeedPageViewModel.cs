using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using OnlabNews.Models;
using OnlabNews.Services.DataSourceServices;

namespace OnlabNews.ViewModels
{
	public class FeedPageViewModel : ViewModelBase
	{

		#region properties

		private IArticleDataSourceService _articleDataSource;
		INavigationService _navigationService;

		public DelegateCommand<object> OnItemClickCommand { get; private set; }

		public ObservableCollection<MutableGrouping<char, ArticleItem>> GroupedArticles { get { return _articleDataSource.GroupedArticles; } }
		
		#endregion

		public FeedPageViewModel(INavigationService navigationService, IArticleDataSourceService dataSourceService)
		{
			_articleDataSource = dataSourceService;
			_navigationService = navigationService;
			OnItemClickCommand = new DelegateCommand<object>(OnClickNavigate);
			
		}

		private void OnClickNavigate(object clickedItem)
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
