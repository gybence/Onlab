using OnlabNews.Extensions;
using OnlabNews.Views;
using Prism.Windows.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnlabNews
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AppShell : Page
	{ 
		ObservableCollection<object> _menuItems;
		public ObservableCollection<object> MenuItems { get => _menuItems; set => _menuItems = value; }

		public AppShell()
		{
			var resourceLoader = ResourceLoader.GetForCurrentView();
			MenuItems = new ObservableCollection<object>
			{
				//new NavItemEx { Content = "Home", Symbol="Home", PageToken="Main", PageType = typeof(MainPage)},	
				new NavItemSeparatorEx(),
				new NavItemHeaderEx {Text = resourceLoader.GetString("Pages"), /*Opacity=0*/},
				new NavItemEx { Content = resourceLoader.GetString("Feeds"), Symbol="List",PageToken="Feed", PageType = typeof(FeedPage), IsStartPage=true},
				new NavItemEx { Content = resourceLoader.GetString("Article"), Symbol="Document",  PageToken="Article", PageType = typeof(ArticlePage)},
				//new NavItemEx { Content = "Dummy page", Symbol="Emoji", PageToken="Settings", PageType = typeof(SettingsPage)},
				//new NavItemSeparatorEx(),
				//new NavItemEx { Text = "My content", Symbol="Folder", Tag="content", PageType = typeof(MainPage)},
			};
			this.InitializeComponent();
			
		}

		public void SetContentFrame(Frame frame, INavigationService service)
		{
			NavView.Setup(frame, service);
			ExtendIntoTitleBar();
		}
		private void ExtendIntoTitleBar()
		{
			var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
			coreTitleBar.ExtendViewIntoTitleBar = true;

			//remove the solid-colored backgrounds behind the caption controls and system back button
			var viewTitleBar = ApplicationView.GetForCurrentView().TitleBar;
			viewTitleBar.ButtonBackgroundColor = Colors.Transparent;
			viewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
			viewTitleBar.ButtonForegroundColor = (Color)Resources["SystemBaseHighColor"];

			Window.Current.CoreWindow.SizeChanged += (s, e) => UpdateAppTitle();
			coreTitleBar.LayoutMetricsChanged += (s, e) => UpdateAppTitle();
		}

		void UpdateAppTitle()
		{
			var full = (ApplicationView.GetForCurrentView().IsFullScreenMode);
			var left = 12 + (full ? 0 : CoreApplication.GetCurrentView().TitleBar.SystemOverlayLeftInset);
				AppTitle.Margin = new Thickness(left, 8, 0, 48);
		}
	}
}