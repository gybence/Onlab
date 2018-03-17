using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using Windows.Web.Syndication;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OnlabNews.Views;
using DataAccessLibrary;
using DataAccessLibrary.Model;
using System.Collections.ObjectModel;
using OnlabNews.Models;
using Microsoft.EntityFrameworkCore;
using Windows.UI.Core;
using OnlabNews.Services.SettingsServices;
using OnlabNews.Services.DataSourceServices;
using Microsoft.Toolkit.Uwp.Services.Facebook;
using System.Globalization;
using OnlabNews.Services.FacebookServices;

namespace OnlabNews.ViewModels
{
	public class SettingsPageViewModel : ViewModelBase
	{

		#region properties

		INavigationService _navigationService;
		private IArticleDataSourceService _articleDataSource;
		private ISettingsService _settingsService;
		private IFacebookGraphService _facebookGraphService;
		private CoreDispatcher dispatcher;


		string _feedNameText;
		public string FeedNameText { get { return _feedNameText; } set { SetProperty(ref _feedNameText, value); } }

		string _feedUriText;
		public string FeedUriText { get { return _feedUriText; } set { SetProperty(ref _feedUriText, value); } }

		string _userNameText;
		public string UserNameText { get { return _userNameText; } set { SetProperty(ref _userNameText, value); } }

		ObservableCollection<RssFeed> _items = new ObservableCollection<RssFeed>();
		public ObservableCollection<RssFeed> Items { get => _items; set { SetProperty(ref _items, value); } }

		#endregion

		public SettingsPageViewModel(INavigationService navigationService, ISettingsService settingsService, IArticleDataSourceService dataSourceService, IFacebookGraphService facebookService)
		{
			_articleDataSource = dataSourceService;
			_settingsService = settingsService;
			_navigationService = navigationService;
			_facebookGraphService = facebookService;


			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
			UserNameText = _settingsService.ActiveUser.Name;
			GetItems();	
		}


		#region button click handlers
		public void AddNewUserButtonClick()
		{
			if (UserNameText.Equals(_settingsService.ActiveUser.Name))
				return;

			using (var db = new AppDbContext())
			{
				User newUser = new User { Name = UserNameText };
				db.Users.Add(newUser);
				db.SaveChanges();
				_settingsService.ActiveUser = newUser;

				GetItems();
			}
		}

		public void LoadButtonClick()
		{
			if (UserNameText.Equals(_settingsService.ActiveUser.Name))
				return;

			using (var db = new AppDbContext())
			{		
				var userToLoad = db.Users.SingleOrDefault(u => u.Name == UserNameText);
				if (userToLoad != null)
				{
					_settingsService.ActiveUser = userToLoad;
					GetItems();
				}
			}
		}

		public void SubButtonClick()
		{
			//UriStrings.Add("https://index.hu/24ora/rss/");
			//UriStrings.Add("https://444.hu/feed");
			//UriStrings.Add("http://rss.cnn.com/rss/edition.rss");
			//UriStrings.Add("http://feeds.bbci.co.uk/news/rss.xml?edition=uk");

			if (!string.IsNullOrEmpty(FeedNameText) && !string.IsNullOrEmpty(FeedUriText))
			{
				using (var db = new AppDbContext())
				{
					//TODO: ha mar letezik ne adjuk hozza 
					var rssItem = new RssFeed { ID = db.RssFeeds.Last().ID + 1 /*lol*/, Name = FeedNameText, Uri = FeedUriText };
					db.RssFeeds.Add(rssItem);
					db.Subscriptions.Add(new Subscription { UserID = _settingsService.ActiveUser.ID, RssFeedID = rssItem.ID });
					db.SaveChanges();
					GetItems();
				}
			}
		}

		private void GetItems()
		{
			Items.Clear();
			using (var db = new AppDbContext())
			{
				var subs = db.Subscriptions.Where(f => f.UserID == _settingsService.ActiveUser.ID).Include(x => x.RssFeed).ToList();
				foreach (Subscription f in subs)
					Items.Add(f.RssFeed);
			}
		}

		#endregion

		public async void FacebookLoginButtonClick()
		{
			if (_facebookGraphService.FacebookServiceInstance.LoggedUser == null)
			{
				await _facebookGraphService.FacebookServiceInstance.LoginAsync();
				_facebookGraphService.LoadFacebookPosts();
			}
			else
				await _facebookGraphService.FacebookServiceInstance.LogoutAsync();
		}

		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(e, viewModelState);
		}
	}
}
