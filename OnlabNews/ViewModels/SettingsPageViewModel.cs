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

		public DelegateCommand<object> OnSubscriptionItemClickCommand { get; private set; }
		public DelegateCommand FacebookLoginCommand { get; private set; }

		#endregion

		public SettingsPageViewModel(INavigationService navigationService, ISettingsService settingsService, IArticleDataSourceService dataSourceService, IFacebookGraphService facebookService)
		{
			_articleDataSource = dataSourceService;
			_settingsService = settingsService;
			_navigationService = navigationService;
			_facebookGraphService = facebookService;


			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
			UserNameText = _settingsService.ActiveUser.Name;

			OnSubscriptionItemClickCommand = new DelegateCommand<object>(OnSubscriptionItemClick);
			FacebookLoginCommand = new DelegateCommand(FacebookLogin);
			GetItems();
		}

		private void OnSubscriptionItemClick(object obj)
		{
			var rssItem = obj as RssFeed;
			using (var db = new AppDbContext())
			{
				var sub = db.Subscriptions.SingleOrDefault(s => s.UserID == _settingsService.ActiveUser.ID && s.RssFeedID == rssItem.ID);
				if(sub == null)
				{
					sub = new Subscription { UserID = _settingsService.ActiveUser.ID, RssFeedID = rssItem.ID };
					db.Subscriptions.Add(sub);
					db.SaveChanges();
					_settingsService.ActiveUser.Subscriptions.Add(sub);
				}
				else
				{
					db.Subscriptions.Remove(sub);
					db.SaveChanges();
					//TODO: valamiert hiaba iratkoztam fel a CollectionChanged eseményre, nem hivodik meg :/
					_settingsService.ActiveUser.Subscriptions.Remove(sub);
				}
			}

		}


		#region button click handlers
		public void AddNewUserButtonClick()
		{
			if (UserNameText.Equals(_settingsService.ActiveUser.Name))
				return;

			User newUser;
			using (var db = new AppDbContext())
			{
				newUser = db.Users.SingleOrDefault(u => u.Name == UserNameText);

				if(newUser == null)
				{
					var currentUser = db.Users.SingleOrDefault(u => u.LastLoggedIn == true);
						currentUser.LastLoggedIn = false;

					newUser = new User { Name = UserNameText, LastLoggedIn = true };
					db.Users.Add(newUser);
					db.SaveChanges();
				}
			}
			_settingsService.ActiveUser = newUser;
			//GetItems();
		}

		public void LoadButtonClick()
		{
			if (UserNameText.Equals(_settingsService.ActiveUser.Name))
				return;

			using (var db = new AppDbContext())
			{
				User userToLoad = db.Users.SingleOrDefault(u => u.Name == UserNameText);

				if (userToLoad != null)
				{
					var currentUser = db.Users.Include(x => x.Subscriptions).SingleOrDefault(u => u.LastLoggedIn == true);
						currentUser.LastLoggedIn = false;

					userToLoad.LastLoggedIn = true;
				
					_settingsService.ActiveUser = userToLoad;
					db.SaveChanges();	
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

		private void GetItems()
		{
			Items.Clear();
			using (var db = new AppDbContext())
			{
				//var subs = db.Subscriptions.Where(f => f.UserID == _settingsService.ActiveUser.ID).Include(x => x.RssFeed).ToList();
				//var subs = db.Subscriptions.Include(x => x.RssFeed).ToList();
				//foreach (Subscription s in subs)
				//	Items.Add(s.RssFeed);

				var feeds = db.RssFeeds.ToList();
				foreach (RssFeed f in feeds)
					Items.Add(f);

			}
		}

		#endregion

		public void FacebookLogin()
		{


		}

		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			//GetItems();
			base.OnNavigatedTo(e, viewModelState);
		}
	}
}
