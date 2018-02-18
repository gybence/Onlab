using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Prism.Commands;
using Windows.Web.Syndication;
using OnlabNews.Models;

namespace OnlabNews.ViewModels
{
	public class MainPageViewModel : ViewModelBase
	{
		#region properties

		INavigationService _navigationService;

		public DelegateCommand OnClickCommand { get; private set; }

		#endregion

		public MainPageViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService;
			OnClickCommand = new DelegateCommand(() => OnClick());
			
		}

		public void OnClick()
		{
			_navigationService.Navigate("Feed", null);
		
			//ContentFrame.
		}
		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(e, viewModelState);
		}



	}
}
