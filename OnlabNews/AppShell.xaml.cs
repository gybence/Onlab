using OnlabNews.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnlabNews
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AppShell : Page
	{
		public AppShell()
		{
			
			this.InitializeComponent();
			
		}
		private void NavView_Loaded(object sender, RoutedEventArgs e)
		{

			// you can also add items in code behind
			NavView.MenuItems.Add(new NavigationViewItemSeparator());
			NavView.MenuItems.Add(new NavigationViewItem()
			{ Content = "My content", Icon = new SymbolIcon(Symbol.Folder), Tag = "content" });

			// set the initial SelectedItem 
			foreach (NavigationViewItemBase item in NavView.MenuItems)
			{
				if (item is NavigationViewItem && item.Tag.ToString() == "home")
				{
					NavView.SelectedItem = item;
					break;
				}
			}
		}

		private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{

			if (args.IsSettingsInvoked)
			{
				ContentFrame.Navigate(typeof(SettingsPage));
			}
			else
			{
				// find NavigationViewItem with Content that equals InvokedItem
				var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
				NavView_Navigate(item as NavigationViewItem);

			}
		}

		private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			if (args.IsSettingsSelected)
			{
				ContentFrame.Navigate(typeof(SettingsPage));
			}
			else
			{
				NavigationViewItem item = args.SelectedItem as NavigationViewItem;
				NavView_Navigate(item);
			}
		}

		private void NavView_Navigate(NavigationViewItem item)
		{
			switch (item.Tag)
			{
				case "home":
					ContentFrame.Navigate(typeof(MainPage));
					break;

				case "apps":
					ContentFrame.Navigate(typeof(FeedPage));
					break;

				case "games":
					ContentFrame.Navigate(typeof(ArticlePage));
					break;

				case "music":
					ContentFrame.Navigate(typeof(SettingsPage));
					break;

				case "content":
					ContentFrame.Navigate(typeof(SettingsPage));
					break;
			}
		}

		public void SetContentFrame(Frame frame)
		{
			//rootSplitView.Content = frame;
			NavView.Setup(frame);
		}
	}
}

