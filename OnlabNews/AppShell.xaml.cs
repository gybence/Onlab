using OnlabNews.Extensions;
using OnlabNews.Views;
using Prism.Windows.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
			MenuItems = new ObservableCollection<object>
			{
				new NavItemEx { Content = "Home", Symbol="Home", PageToken="Main", PageType = typeof(MainPage), IsStartPage=true},	
				new NavItemSeparatorEx(),
				new NavItemHeaderEx {Text="Pages", /*Opacity=0*/},
				new NavItemEx { Content = "Feed page", Symbol="List",PageToken="Feed", PageType = typeof(FeedPage)},
				new NavItemEx { Content = "Article page", Symbol="Document",  PageToken="Article", PageType = typeof(ArticlePage)},
				new NavItemEx { Content = "Dummy page", Symbol="Emoji", PageToken="Settings", PageType = typeof(SettingsPage)},
				//new NavItemSeparatorEx(),
				//new NavItemEx { Text = "My content", Symbol="Folder", Tag="content", PageType = typeof(MainPage)},
			};
			this.InitializeComponent();
			
		}

		public void SetContentFrame(Frame frame, INavigationService service)
		{
			NavView.Setup(frame, service);
		}
	}
}