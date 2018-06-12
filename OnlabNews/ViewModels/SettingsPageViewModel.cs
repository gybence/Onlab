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
using OnlabNews.Models;
using System;
using Windows.UI.Xaml;

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

		private bool _changesHappened;
		public bool ChangesHappened { get => _changesHappened; set { SetProperty(ref _changesHappened, value); } }

		ObservableCollection<RssFeed> _items = new ObservableCollection<RssFeed>();
		public ObservableCollection<RssFeed> Items { get => _items; set { SetProperty(ref _items, value); } }

		private ObservableCollection<BooleanWithIndex> _itemsBool = new ObservableCollection<BooleanWithIndex>();
		public ObservableCollection<BooleanWithIndex> ItemsBool { get => _itemsBool; set { SetProperty(ref _itemsBool, value); } }

		private List<Subscription> _subEdits;
		

		public DelegateCommand<object> OnSubscriptionItemClickCommand { get; private set; }
		public DelegateCommand FacebookLoginCommand { get; private set; }
		public DelegateCommand FacebookLogoutCommand { get; private set; }
		//public DelegateCommand<object> TappedEventHandlerCommand { get; private set; }

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
			//TappedEventHandlerCommand = new DelegateCommand<object>(TappedEventHandler);

		}


		#region click handlers

		public void SubButtonClick()
		{
			//peldak:
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
						var bwi = new BooleanWithIndex(false,ItemsBool.Count);
						ItemsBool.Add(bwi);
					}
					db.SaveChanges();
				}
			}
		}
		//public void TappedEventHandler(object obj)
		//{

		//}
		public void OnSubscriptionItemClick(object obj)
		{
			int index = (int)obj;
			try
			{
				var rssItem = Items[index];
				using (var db = new AppDbContext())
				{
					var sub = _subEdits.FirstOrDefault(x => x.RssFeedID == rssItem.ID);
					//var sub = db.Subscriptions.Include(x => x.User).SingleOrDefault(s => s.UserID == _settingsService.ActiveUser.ID && s.RssFeedID == rssItem.ID);
					if (sub == null)
					{
						sub = new Subscription { UserID = _settingsService.ActiveUser.ID, RssFeedID = rssItem.ID };

						_subEdits.Add(sub);
						ItemsBool[index].Value = true;
					}
					else
					{
						_subEdits.Remove(sub);
						ItemsBool[index].Value = false;
					}
				}
				ChangesHappened = true;
			}
			catch(Exception)
			{

			}
			
		}

		public async void SaveButtonClick()
		{
			ChangesHappened = false;
			using (var db = new AppDbContext())
			{

				//kidobjuk a db-bol ami nincs benne az uj listaban
				foreach (var s in db.Subscriptions)
				{
					if (s.UserID == _settingsService.ActiveUser.ID && !_subEdits.Exists(x => x.SubscriptionID == s.SubscriptionID))
						db.Subscriptions.Remove(s);
				}
				//hozza kell adni a listabol azokat amik meg nincsenek a db-ben
				db.Subscriptions.AddRange(_subEdits.Where(x => !db.Subscriptions.Any(y => y.SubscriptionID == x.SubscriptionID)));

				db.SaveChanges();
				//active user, settings es db szinkronizalasa
				_settingsService.ActiveUser.Subscriptions = db.Users.FirstOrDefault(x => x.ID == _settingsService.ActiveUser.ID).Subscriptions;
				//_subscriptionModificationList = new List<Subscription>(_settingsService.ActiveUser.Subscriptions);

			}

			await _articleDataSource.CreateArticlesAsync();
		}

		private void GetItems()
		{
			Items.Clear();
			ItemsBool.Clear();
			using (var db = new AppDbContext())
			{
				var feeds = db.RssFeeds.ToList();
				foreach (RssFeed f in feeds)
				{
					Items.Add(f);
					if (_subEdits.Exists(x => x.RssFeedID == f.ID))
					{
						var bwi = new BooleanWithIndex(true, ItemsBool.Count);
						ItemsBool.Add(bwi);
					}
					else
					{
						var bwi = new BooleanWithIndex(false, ItemsBool.Count);
						ItemsBool.Add(bwi);
					}
				}

			}
		}

		private async void FacebookLogin()
		{
			bool success = await _facebookService.SignInFacebookAsync();


			if (success)
			{
				UserNameText = _facebookService.UserName;

				using (var db = new AppDbContext())
				{
					User userToLoad = db.Users.Include(x => x.Subscriptions).SingleOrDefault(u => u.Name == _facebookService.UserID);
					if (userToLoad == null)
					{
						var currentUser = db.Users.SingleOrDefault(u => u.LastLoggedIn == true);
						currentUser.LastLoggedIn = false;


						userToLoad = new User { Name = _facebookService.UserID, LastLoggedIn = true };
						db.Users.Add(userToLoad);
						_settingsService.ActiveUser = userToLoad;
						db.SaveChanges();
					}
					else
					{
						var currentUser = db.Users.SingleOrDefault(u => u.LastLoggedIn == true);
						currentUser.LastLoggedIn = false;

						userToLoad.LastLoggedIn = true;

						_settingsService.ActiveUser = userToLoad;

						db.SaveChanges();
					}
				}
				_subEdits = new List<Subscription>(_settingsService.ActiveUser.Subscriptions);
				ChangesHappened = false;
				SyncTickBoxes();
				await _articleDataSource.CreateArticlesAsync();
			}
		}
		private async void FacebookLogout()
		{
			bool success = await _facebookService.SignOutFacebookAsync();

			if (success)
			{
				UserNameText = null;
				using (var db = new AppDbContext())
				{
					User userToLoad = db.Users.Include(x => x.Subscriptions).SingleOrDefault(u => u.Name == _facebookService.UserID);
					var currentUser = db.Users.SingleOrDefault(u => u.LastLoggedIn == true);

					currentUser.LastLoggedIn = false;
					userToLoad.LastLoggedIn = true;

					_settingsService.ActiveUser = userToLoad;

					db.SaveChanges();
				}
				_subEdits = new List<Subscription>(_settingsService.ActiveUser.Subscriptions);
				ChangesHappened = false;
				SyncTickBoxes();
				await _articleDataSource.CreateArticlesAsync();
			}
		}

		#endregion
		private void SyncTickBoxes()
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (_subEdits.Exists(x => x.RssFeedID == Items[i].ID))
					ItemsBool[i].Value = true;
				else
					ItemsBool[i].Value = false;
			}
		}
		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			if (_facebookService.UserID == "Default")
				UserNameText = null;
			else
				UserNameText = _facebookService.UserName;

			_subEdits = new List<Subscription>(_settingsService.ActiveUser.Subscriptions);



			ChangesHappened = false;
			GetItems();
			base.OnNavigatedTo(e, viewModelState);
		}
	}
}
