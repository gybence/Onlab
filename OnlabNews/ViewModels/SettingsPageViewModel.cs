using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System.Collections.Generic;
using System.Linq;
using DataAccessLibrary;
using DataAccessLibrary.Model;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Windows.UI.Core;
using OnlabNews.Services.SettingsServices;
using OnlabNews.Services.DataSourceServices;
using OnlabNews.Services.FacebookServices;

namespace OnlabNews.ViewModels
{
	public class SettingsPageViewModel : ViewModelBase
	{

		#region properties

		INavigationService _navigationService;
		private IArticleDataSourceService _articleDataSource;
		private ISettingsService _settingsService;
		private IFacebookService _facebookService;
		private CoreDispatcher dispatcher;


		string _feedNameText;
		public string FeedNameText { get { return _feedNameText; } set { SetProperty(ref _feedNameText, value); } }

		string _feedUriText;
		public string FeedUriText { get { return _feedUriText; } set { SetProperty(ref _feedUriText, value); } }

		string _userNameText;
		public string UserNameText { get { return _userNameText; } set { SetProperty(ref _userNameText, value); } }

		ObservableCollection<RssFeed> _items = new ObservableCollection<RssFeed>();
		public ObservableCollection<RssFeed> Items { get => _items; set { SetProperty(ref _items, value); } }

		private List<Subscription> _subscriptionModificationList;

		public DelegateCommand<object> OnSubscriptionItemClickCommand { get; private set; }
		public DelegateCommand FacebookLoginCommand { get; private set; }
		public DelegateCommand FacebookLogoutCommand { get; private set; }
		
		#endregion

		public SettingsPageViewModel(INavigationService navigationService, 
									 ISettingsService settingsService, 
									 IArticleDataSourceService dataSourceService, 
									 IFacebookService facebookService)
		{
			_articleDataSource = dataSourceService;
			_settingsService = settingsService;
			_navigationService = navigationService;
			_facebookService = facebookService;
			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
	
			OnSubscriptionItemClickCommand = new DelegateCommand<object>(OnSubscriptionItemClick);
			FacebookLoginCommand = new DelegateCommand(FacebookLogin);
			FacebookLogoutCommand = new DelegateCommand(FacebookLogout);
			GetItems();
		}

		
		#region click handlers

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
					var rssItem = db.RssFeeds.SingleOrDefault(f => f.Uri == FeedUriText);

					if (rssItem == null)
					{
						rssItem = new RssFeed { ID = db.RssFeeds.Last().ID + 1 /*lol*/, Name = FeedNameText, Uri = FeedUriText };
						db.RssFeeds.Add(rssItem);
						Items.Add(rssItem);
					}
					db.SaveChanges();
				}
			}
		}

		private void OnSubscriptionItemClick(object obj)
		{
			var rssItem = obj as RssFeed;
			using (var db = new AppDbContext())
			{
				var sub = _subscriptionModificationList.FirstOrDefault(x => x.RssFeedID == rssItem.ID);
				//var sub = db.Subscriptions.Include(x => x.User).SingleOrDefault(s => s.UserID == _settingsService.ActiveUser.ID && s.RssFeedID == rssItem.ID);
				if (sub == null)
				{
					sub = new Subscription { UserID = _settingsService.ActiveUser.ID, RssFeedID = rssItem.ID };

					_subscriptionModificationList.Add(sub);
					//db.Subscriptions.Add(sub);
					//db.SaveChanges();
					//_settingsService.ActiveUser.Subscriptions.Add(sub);
				}
				else
				{
					_subscriptionModificationList.Remove(sub);
					//foreach(Subscription s in _subscriptionModificationList)
					//{
					//	if(s.SubscriptionID == sub.SubscriptionID)
					//	{
					//		_subscriptionModificationList.Remove(s);
					//		break;
					//	}
					//}
					//db.Subscriptions.Remove(sub);
					//db.SaveChanges();
					//_settingsService.ActiveUser.Subscriptions.Remove(sub);
				}
			}
			//await _articleDataSource.CreateArticlesAsync();
		}

		public async void SaveButtonClick()
		{
			using (var db = new AppDbContext())
			{
				var userID = _settingsService.ActiveUser.ID;
				
				//kidobjuk a db-bol ami nincs benne az uj listaban
				foreach (var s in db.Subscriptions)
				{
					if (s.UserID == userID && !_subscriptionModificationList.Exists(x=> x.SubscriptionID == s.SubscriptionID))
						db.Subscriptions.Remove(s);
				}
				//hozza kell adni a listabol azokat amik meg nincsenek a db-ben
				db.Subscriptions.AddRange(_subscriptionModificationList.Where(x => !db.Subscriptions.Any(y => y.SubscriptionID == x.SubscriptionID)));
				
				db.SaveChanges();
				//active user, settings es db szinkronizalasa
				_settingsService.ActiveUser.Subscriptions = db.Users.FirstOrDefault(x => x.ID == _settingsService.ActiveUser.ID).Subscriptions;
				_subscriptionModificationList = new List<Subscription>(_settingsService.ActiveUser.Subscriptions);

			}
			await _articleDataSource.CreateArticlesAsync();
		}

		private void GetItems()
		{
			Items.Clear();
			using (var db = new AppDbContext())
			{
				var feeds = db.RssFeeds.ToList();
				foreach (RssFeed f in feeds)
					Items.Add(f);
			}
		}	

		public async void FacebookLogin()
		{
			bool success = await _facebookService.SignInFacebookAsync();


			if (success)
			{
				UserNameText = _facebookService.UserID;

				using (var db = new AppDbContext())
				{
					User userToLoad = db.Users.SingleOrDefault(u => u.Name == _facebookService.UserID);
					if (userToLoad == null)
					{
						var currentUser = db.Users.SingleOrDefault(u => u.LastLoggedIn == true);
						currentUser.LastLoggedIn = false;

						userToLoad = new User { Name = UserNameText, LastLoggedIn = true };
						db.Users.Add(userToLoad);
						db.SaveChanges();
					}
					else
					{
						var currentUser = db.Users.Include(x => x.Subscriptions).SingleOrDefault(u => u.LastLoggedIn == true);
						currentUser.LastLoggedIn = false;

						userToLoad.LastLoggedIn = true;

						_settingsService.ActiveUser = userToLoad;
						db.SaveChanges();
					}
				}
				await _articleDataSource.CreateArticlesAsync();
			}
		}
		public async void FacebookLogout()
		{
			bool success = await _facebookService.SignOutFacebookAsync();

			if (success)
			{
				UserNameText = null;
				using (var db = new AppDbContext())
				{
					User userToLoad = db.Users.SingleOrDefault(u => u.Name == _facebookService.UserID);
					var currentUser = db.Users.Include(x => x.Subscriptions).SingleOrDefault(u => u.LastLoggedIn == true);
					currentUser.LastLoggedIn = false;

					userToLoad.LastLoggedIn = true;

					_settingsService.ActiveUser = userToLoad;
					db.SaveChanges();
				}
				await _articleDataSource.CreateArticlesAsync();
			}
		}

		#endregion

		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			if (_facebookService.UserID == "Default")
				UserNameText = null;
			else
				UserNameText = _facebookService.UserID;
			_subscriptionModificationList = new List<Subscription>(_settingsService.ActiveUser.Subscriptions);
			//GetItems();
			base.OnNavigatedTo(e, viewModelState);
		}
	}
}
