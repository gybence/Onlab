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

namespace OnlabNews.Extensions
{
	public class NavViewEx : NavigationView
	{
		#region properties

		INavigationService _navigationService;
		Frame _frame;
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
						//Navigate(_frame, i.GetValue(NavProperties.PageTypeProperty) as Type);
						_navigationService.Navigate(i.Tag.ToString(), null);
						base.SelectedItem = value;
						//_frame.BackStack.Clear();
					}
				}
				UpdateBackButton();
			}
		}

		#endregion


		public NavViewEx()
		{
			//Content = _frame = new Frame();
			//_frame.Navigated += Frame_Navigated;
			//ItemInvoked += NavViewEx_ItemInvoked;
			//SystemNavigationManager.GetForCurrentView()
			//  .BackRequested += ShellPage_BackRequested;
		}


		public void Setup(Frame frame, INavigationService service)
		{			
			Content = _frame = frame;
			_navigationService = service;
			_frame.Navigated += Frame_Navigated;
			ItemInvoked += NavViewEx_ItemInvoked;
			//SystemNavigationManager.GetForCurrentView().BackRequested += ShellPage_BackRequested;


			Loaded += (s, e) =>
			{
				if (FindStart() is NavigationViewItem i && i != null)
					_navigationService.Navigate(i.Tag.ToString(), null);
					//Navigate(_frame, i.GetValue(NavProperties.PageTypeProperty) as Type);
			};
			
			RegisterPropertyChangedCallback(IsPaneOpenProperty, IsPaneOpenChanged);
		}


		private void NavViewEx_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			if (args.IsSettingsInvoked)
				SelectedItem = SettingsItem;
			else
				SelectedItem = Find(args.InvokedItem.ToString());
		}


		private void Frame_Navigated(object sender, NavigationEventArgs e)
		  => SelectedItem = (e.SourcePageType == SettingsPageType)
			 ? SettingsItem : Find(e.SourcePageType) ?? base.SelectedItem;


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

		
		private void IsPaneOpenChanged(DependencyObject sender,
		  DependencyProperty dp)
		{
			foreach (var item in MenuItems.OfType<NavigationViewItemHeader>())
			{
				item.Opacity = IsPaneOpen ? 1 : 0;
			}
		}
	}
}
