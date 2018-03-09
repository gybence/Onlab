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

namespace OnlabNews.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
		
		#region properties

		INavigationService _navigationService;
		private IArticleDataSourceService _articleDataSource;
		private ISettingsService _settingsService;
		private CoreDispatcher dispatcher;

		

		string _nameToLoadText;
		public string NameToLoadText { get { return _nameToLoadText; } set { SetProperty(ref _nameToLoadText, value); } }

		string _feedNameText;
		public string FeedNameText { get { return _feedNameText; } set { SetProperty(ref _feedNameText, value); } }

		string _feedUriText;
		public string FeedUriText { get { return _feedUriText; } set { SetProperty(ref _feedUriText, value); } }

		string _userNameText;
		public string UserNameText { get { return _userNameText; } set { SetProperty(ref _userNameText, value); } }

		ObservableCollection<RssFeed> _items = new ObservableCollection<RssFeed>();
		public ObservableCollection<RssFeed> Items { get => _items; set { SetProperty(ref _items, value); } }

		#endregion

		public SettingsPageViewModel(INavigationService navigationService, ISettingsService settingsService, IArticleDataSourceService dataSourceService)
		{
			_articleDataSource = dataSourceService;
			_settingsService = settingsService;
			_navigationService = navigationService;
			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
			NameToLoadText = _settingsService.ActiveUser.Name;
			GetItems();
		}

		public void AddNewUserButtonClick()
		{
			using (var db = new AppDbContext())
			{
				try
				{
					var userToLoad = db.Users.SingleOrDefault(u => u.Name == UserNameText);

					if (userToLoad == null)
					{
						userToLoad = new User { Name = UserNameText };
						db.Users.Add(userToLoad);
						db.SaveChanges();
						_settingsService.ActiveUser = userToLoad;
						NameToLoadText = UserNameText;
						GetItems();

					}
				}
				catch (Exception e)
				{
					System.Diagnostics.Debugger.Break();
				}
			}
		}

		public void LoadButtonClick()
		{
			if (!string.Equals(NameToLoadText, _settingsService.ActiveUser.Name))
			{
				using (var db = new AppDbContext())
				{
					try
					{
						var userToLoad = db.Users.SingleOrDefault(u => u.Name == NameToLoadText);
						if (userToLoad != null)
						{
							_settingsService.ActiveUser = userToLoad;
							GetItems();

						}
					}
					catch (Exception e)
					{
						System.Diagnostics.Debugger.Break();
					}
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
				try
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
				catch (Exception e)
				{
					System.Diagnostics.Debugger.Break();
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
				{
					Items.Add(f.RssFeed);
				}

			}
		}

		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(e, viewModelState);
		}
	}
}
