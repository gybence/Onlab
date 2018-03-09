using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using OnlabNews.Extensions;

namespace OnlabNews.Controls
{
	public class NavViewEx : NavigationView
	{
		#region properties

		INavigationService _navigationService;
		Frame _frame; //arra kell hogy ne kelljen allandoan castolgatni a Content-t Frame-e
		public Type SettingsPageType { get; set; }

		public new object SelectedItem
		{
			set
			{
				if (base.SelectedItem != value)
				{
					if (value == SettingsItem)
					{
						//Navigate(_frame, SettingsPageType);
						_navigationService.Navigate("Settings", null);
						base.SelectedItem = value;
						//_frame.BackStack.Clear();
					}
					else if (value is NavigationViewItem i && i != null)
					{
						string targetPageToken = i.GetValue(NavProperties.PageTokenProperty).ToString();
						//Navigate(_frame, i.GetValue(NavProperties.PageTypeProperty) as Type);
						_navigationService.Navigate(targetPageToken, null);
						base.SelectedItem = value;
						if (targetPageToken.Equals("Main"))
						{
							//_frame.BackStack.Clear();
							_navigationService.RemoveAllPages();
							_navigationService.ClearHistory();
						}
					}
				}
				UpdateBackButton();
				UpdateHeader();
			}
			get { return base.SelectedItem; }
		}

		#endregion


		public NavViewEx()
		{
		}


		public void Setup(Frame frame, INavigationService service)
		{
			//frame.Margin = new Thickness(24);
			frame.Margin = new Thickness(24, 24, 0, 24);
			Content = _frame = frame;
			_navigationService = service;
			_frame.Navigated += Frame_Navigated; //navigacio navview nelkul
			ItemInvoked += NavViewEx_ItemInvoked; //navigacio navview menu itemre kattintassal 
			Loaded += NavViewEx_Loaded;
			DisplayModeChanged += MyDisplayModeChanged;
			//SystemNavigationManager.GetForCurrentView().BackRequested += ShellPage_BackRequested;
		

			RegisterPropertyChangedCallback(IsPaneOpenProperty, IsPaneOpenChanged);
			
		}
		
			private void NavViewEx_Loaded(object sender, RoutedEventArgs e)
		{
			if (FindStart() is NavigationViewItem i && i != null)
			{
				_navigationService.Navigate(i.GetValue(NavProperties.PageTokenProperty).ToString(),null);
			}
			IsPaneOpenChanged(this, null);
			UpdateBackButton();
			UpdateHeader();
		}

		private void NavViewEx_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			if (args.IsSettingsInvoked)
				SelectedItem = SettingsItem;
			else
				SelectedItem = Find(args.InvokedItem.ToString());
		}


		private void Frame_Navigated(object sender, NavigationEventArgs e)
		{
			if (e.SourcePageType == SettingsPageType)
				SelectedItem = SettingsItem;
			else
				SelectedItem = Find(e.SourcePageType) ?? base.SelectedItem;
		}


		//private void ShellPage_BackRequested(object sender, BackRequestedEventArgs e)
		//{

		//	if (_navigationService.CanGoBack())
		//		_navigationService.GoBack();
		//}


		NavigationViewItem FindStart()
			  => MenuItems.OfType<NavigationViewItem>()
				.SingleOrDefault(x => (bool)x.GetValue(NavProperties.IsStartPageProperty));


		NavigationViewItem Find(string content)
		  => MenuItems.OfType<NavigationViewItem>()
			.SingleOrDefault(x => x.Content.Equals(content));


		NavigationViewItem Find(Type type)
		  => MenuItems.OfType<NavigationViewItem>()
			.SingleOrDefault(x => type.Equals(x.GetValue(NavProperties.PageTypeProperty)));


		//public virtual void Navigate(Frame frame, Type type)
		//  => frame.Navigate(type); 


		private void UpdateBackButton()
		{
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
			  (_navigationService.CanGoBack()) ? AppViewBackButtonVisibility.Visible
				: AppViewBackButtonVisibility.Collapsed;
		}

		public enum HeaderBehaviors { Hide, Remove, None }

		public HeaderBehaviors HeaderBehavior { get; set; } = HeaderBehaviors.Remove;
		private void IsPaneOpenChanged(DependencyObject sender, DependencyProperty dp)
		{
			foreach (var item in MenuItems.OfType<NavigationViewItemHeader>())
			{
				switch (HeaderBehavior)
				{
					case HeaderBehaviors.Hide:
						item.Opacity = IsPaneOpen ? 1 : 0;
						break;
					case HeaderBehaviors.Remove:
						item.Visibility = IsPaneOpen ? Visibility.Visible : Visibility.Collapsed;
						break;
					case HeaderBehaviors.None:
						// empty
						break;
				}
			}
		}

		private void UpdateHeader()
		{
			if (_frame.Content is Page p && p.GetValue(NavProperties.HeaderProperty) is string s && !string.IsNullOrEmpty(s))
			{
				Header = s;
			}
		}

		//We recommend 12px margins for your content area when NavigationView is in Minimal mode and 24px margins otherwise.
		private void MyDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs e)
		{
			if (DisplayMode == NavigationViewDisplayMode.Minimal)
				_frame.Margin = new Thickness(12, 12, 0, 12);
			else
				_frame.Margin = new Thickness(24, 24, 0, 24);
		}
	}
}
