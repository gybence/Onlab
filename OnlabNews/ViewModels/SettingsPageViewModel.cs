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

namespace OnlabNews.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
		#region properties

		INavigationService _navigationService;

		private CoreDispatcher dispatcher;

		public Settings Settings
		{
			get { return Settings.Instance; }
		}

		string _nameToLoadText;
		public string NameToLoadText { get { return _nameToLoadText; } set { SetProperty(ref _nameToLoadText, value); } }

		string _feedNameText;
		public string FeedNameText { get { return _feedNameText; } set { SetProperty(ref _feedNameText, value); } }

		string _feedUriText;
		public string FeedUriText { get { return _feedUriText; } set { SetProperty(ref _feedUriText, value); } }

		string _userNameText;
		public string UserNameText { get { return _userNameText; } set { SetProperty(ref _userNameText, value); } }

		ObservableCollection<RssItem> _items = new ObservableCollection<RssItem>();
		public ObservableCollection<RssItem> Items { get => _items; set { SetProperty(ref _items, value); } }

		#endregion

		public SettingsPageViewModel(INavigationService navigationService) 
		{
			_navigationService = navigationService;
			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
			NameToLoadText = Settings.ActiveUser.Name;
			GetItems();
		}

		public async Task AddNewUserButtonClickAsync()
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
						Settings.ActiveUser = userToLoad;
						NameToLoadText = UserNameText;
						GetItems();
						//TODO: nem biztos hogy ezt itt kene 1
						await dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
						{
							await ItemDataSource.Instance.QueryArticles();
						});
					}
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine(e.InnerException.Message);
					System.Diagnostics.Debugger.Break();
				}
			}
		}

		public async Task LoadButtonClickAsync()
		{
			if (!string.Equals(NameToLoadText, Settings.ActiveUser.Name))
			{
				using (var db = new AppDbContext())
				{
					try
					{
						var userToLoad = db.Users.SingleOrDefault(u => u.Name == NameToLoadText);
						if (userToLoad != null)
						{
							Settings.ActiveUser = userToLoad;
							GetItems();
							//TODO: nem biztos hogy ezt itt kene 2
							await dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
							{
								await ItemDataSource.Instance.QueryArticles();
							});
						}
					}
					catch(Exception e)
					{
						System.Diagnostics.Debug.WriteLine(e.InnerException.Message);
						System.Diagnostics.Debugger.Break();
					}
				}
			}
		}

		public async Task SubButtonClickAsync()
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
						var rssItem = new RssItem { ID = db.RssItems.Last().ID + 1 /*lol*/, Name = FeedNameText, Uri = FeedUriText };
						db.RssItems.Add(rssItem);
						db.Follows.Add(new Follow { UserID = Settings.ActiveUser.ID, RssItemID = rssItem.ID });
						db.SaveChanges();

						//TODO: nem biztos hogy ezt itt kene 3
						await dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
						{
							await ItemDataSource.Instance.QueryArticles();
						});

						GetItems();

					}
				}catch(Exception e)
				{
					System.Diagnostics.Debug.WriteLine(e.InnerException.Message);
					System.Diagnostics.Debugger.Break();
				}
			}
		}

		private void GetItems()
		{
			Items.Clear();
			using (var db = new AppDbContext())
			{

				var follows = db.Follows.Where(f => f.UserID == Settings.ActiveUser.ID).Include(x => x.RssItem).ToList();

				foreach (Follow f in follows)
				{
					Items.Add(f.RssItem);
				}

			}
		}

		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(e, viewModelState);
		}
	}
}
