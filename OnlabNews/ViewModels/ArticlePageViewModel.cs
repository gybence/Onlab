using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlabNews.Views;

namespace OnlabNews.ViewModels
{
	public class ArticlePageViewModel: ViewModelBase
	{
		#region properties

		private INavigationService _navigationService;


		string _articleLink;
		public string ArticleLink { get => _articleLink; set { SetProperty(ref _articleLink, value); } }

		#endregion

		public ArticlePageViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService;	
		}
		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			ArticleLink = (e.Parameter as ArticleItem).Uri;
			base.OnNavigatedTo(e, viewModelState);
			
		}
	}
}
