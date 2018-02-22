﻿using Prism.Windows.Mvvm;
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
using DataAccessLibrary;

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

			Task.Run(ItemDataSource.Instance.QueryArticles);
		}

		public void OnClick()
		{
			//_navigationService.Navigate("Feed", null);

			//using (var ctx = new AppContext())
			//{
			//	var user = new User() { UserName="Dummyboii"};

			//	ctx.Users.Add(user);
			//	try
			//	{
			//		ctx.SaveChanges();
			//	}catch(Exception e)
			//	{
			//	}
			//}

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
