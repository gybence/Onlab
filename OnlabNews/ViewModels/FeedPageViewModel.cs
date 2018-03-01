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
using Windows.UI.Xaml.Controls;

namespace OnlabNews.ViewModels
{
	public class FeedPageViewModel : ViewModelBase
	{
		#region properties

		INavigationService _navigationService;

		public ItemDataSource ItemDataSource
		{
			get { return ItemDataSource.Instance; }
		}

		#endregion

		public FeedPageViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService;
		}

		public void ArticleClick(object sender, ItemClickEventArgs e)
		{
			var item = e.ClickedItem;
			_navigationService.Navigate("Article", item);
			//_navigationService.Navigate("Settings",null);
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
