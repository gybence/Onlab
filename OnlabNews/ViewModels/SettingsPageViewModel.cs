using DataAccessLibrary;
using DataAccessLibrary.Model;
using Microsoft.EntityFrameworkCore;
using OnlabNews.Models.DTOs;
using OnlabNews.Services.DataSourceServices;
using OnlabNews.Services.FacebookServices;
using OnlabNews.Services.SettingsServices;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Core;

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

		private bool _settingsModified;
		public bool SettingsModified { get => _settingsModified; set { SetProperty(ref _settingsModified, value); } }

		ObservableCollection<RssFeedDTO> _items = new ObservableCollection<RssFeedDTO>();
		public ObservableCollection<RssFeedDTO> Items { get => _items; set { SetProperty(ref _items, value); } }

		private List<Subscription> _subEdits;


		public DelegateCommand<object> OnSubscriptionItemClickCommand { get; private set; }
		public DelegateCommand<object> OnEditRowButtonClickCommand { get; private set; }
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
			OnEditRowButtonClickCommand = new DelegateCommand<object>(OnEditRowButtonClick);
			FacebookLoginCommand = new DelegateCommand(FacebookLogin);
			FacebookLogoutCommand = new DelegateCommand(FacebookLogout);
		}

		#region click handlers

		private void OnEditRowButtonClick(object obj)
		{
			RssFeedDTO rssFeedDTO = obj as RssFeedDTO;
			if (rssFeedDTO.IsReadOnly)
			{
				rssFeedDTO.IsReadOnly = false;
			}
			else
			{
				rssFeedDTO.IsReadOnly = true;
				SettingsModified = true;

			}
		}

		public void AddNewRssSubscribtionButtonClick()
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
						var item = new RssFeedDTO(rssItem, false, Items.Count);
						Items.Add(item);
					}
					db.SaveChanges();
					FeedNameText = "";
					FeedUriText = "";
				}
			}
		}

		private void OnSubscriptionItemClick(object obj)
		{
			RssFeedDTO rssFeedDTO = obj as RssFeedDTO;

			using (var db = new AppDbContext())
			{
				var sub = _subEdits.FirstOrDefault(x => x.RssFeedID == rssFeedDTO.RssFeed.ID);
				if (sub == null)
				{
					sub = new Subscription { UserID = _settingsService.ActiveUser.ID, RssFeedID = rssFeedDTO.RssFeed.ID };
					_subEdits.Add(sub);
					rssFeedDTO.Enabled = true;
				}
				else
				{
					_subEdits.Remove(sub);
					rssFeedDTO.Enabled = false;
				}
			}
			SettingsModified = true;
		}

		public async void SaveButtonClick()
		{
			SettingsModified = false;
			using (var db = new AppDbContext())
			{
				foreach (var fdto in Items)
				{
					var f = db.RssFeeds.FirstOrDefault(x => x.ID == fdto.RssFeed.ID);
					if (f.Name != fdto.RssFeed.Name)
						f.Name = fdto.RssFeed.Name;
					if (f.Uri != fdto.RssFeed.Uri)
						f.Uri = fdto.RssFeed.Uri;
				}
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
			using (var db = new AppDbContext())
			{
				var feeds = db.RssFeeds.ToList();
				foreach (RssFeed f in feeds)
				{
					var item = new RssFeedDTO(f, _subEdits.Exists(x => x.RssFeedID == f.ID), Items.Count);
					Items.Add(item);
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
						db.Users.SingleOrDefault(u => u.LastLoggedIn == true).LastLoggedIn = false;
						userToLoad = new User { Name = _facebookService.UserID, LastLoggedIn = true };
						db.Users.Add(userToLoad);
						_settingsService.ActiveUser = userToLoad;
						db.SaveChanges();
					}
					else
					{
						var currentUser = db.Users.SingleOrDefault(u => u.LastLoggedIn == true).LastLoggedIn = false;
						userToLoad.LastLoggedIn = true;
						_settingsService.ActiveUser = userToLoad;
						db.SaveChanges();
					}
				}
				_subEdits = new List<Subscription>(_settingsService.ActiveUser.Subscriptions);
				SettingsModified = false;
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
				SettingsModified = false;
				SyncTickBoxes();
				await _articleDataSource.CreateArticlesAsync();
			}
		}

		#endregion
		private void SyncTickBoxes()
		{
			foreach (RssFeedDTO i in Items)
			{
				i.Enabled = _subEdits.Exists(x => x.RssFeedID == i.RssFeed.ID); ;
			}
		}
		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			if (_facebookService.UserID == "Default")
				UserNameText = null;
			else
				UserNameText = _facebookService.UserName;

			_subEdits = new List<Subscription>(_settingsService.ActiveUser.Subscriptions);

			SettingsModified = false;
			GetItems();
			base.OnNavigatedTo(e, viewModelState);
		}
	}
}
