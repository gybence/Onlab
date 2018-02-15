using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.ViewModels
{
	public class ArticlePageViewModel: ViewModelBase
	{
		private INavigationService _navigationService;


		private string _articleLink;
		public string ArticleLink { get => _articleLink; set { SetProperty(ref _articleLink, value); } }


		public ArticlePageViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService;
	
		
		}
		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			
			base.OnNavigatedTo(e, viewModelState);
			ArticleLink = e.Parameter.ToString();
		}
	}
}
