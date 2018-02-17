using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace OnlabNews.Extensions
{
	public class NavViewEx : NavigationView
	{
		Frame _frame;
		public Type SettingsPageType { get; set; }
		public NavViewEx()
		{
			//Content = _frame = new Frame();
			//_frame.Navigated += Frame_Navigated;
			//ItemInvoked += NavViewEx_ItemInvoked;
			//SystemNavigationManager.GetForCurrentView()
			//  .BackRequested += ShellPage_BackRequested;
		}
		public void Setup(Frame frame)
		{
			Content = _frame = frame;
			_frame.Navigated += Frame_Navigated;
			ItemInvoked += NavViewEx_ItemInvoked;
			SystemNavigationManager.GetForCurrentView()
			  .BackRequested += ShellPage_BackRequested;
		}
		private void NavViewEx_ItemInvoked(NavigationView sender,
		  NavigationViewItemInvokedEventArgs args)
		{
			if (args.IsSettingsInvoked)
				SelectedItem = SettingsItem;
			else
				SelectedItem = Find(args.InvokedItem.ToString());
		}
		private void Frame_Navigated(object sender, NavigationEventArgs e)
		  => SelectedItem = (e.SourcePageType == SettingsPageType)
			 ? SettingsItem : Find(e.SourcePageType) ?? base.SelectedItem;
		private void ShellPage_BackRequested(object sender, BackRequestedEventArgs e)
		  => _frame.GoBack();
		NavigationViewItem Find(string content)
		  => MenuItems.OfType<NavigationViewItem>()
			.SingleOrDefault(x => x.Content.Equals(content));
		NavigationViewItem Find(Type type)
		  => MenuItems.OfType<NavigationViewItem>()
			.SingleOrDefault(x => type.Equals(x.GetValue(NavProperties.PageTypeProperty)));
		public virtual void Navigate(Frame frame, Type type)
		  => frame.Navigate(type);
		public new object SelectedItem
		{
			set
			{
				if (value == SettingsItem)
				{
					Navigate(_frame, SettingsPageType);
					base.SelectedItem = value;
					_frame.BackStack.Clear();
				}
				else if (value is NavigationViewItem i && i != null)
				{
					Navigate(_frame, i.GetValue(NavProperties.PageTypeProperty) as Type);
					base.SelectedItem = value;
					_frame.BackStack.Clear();
				}
				UpdateBackButton();
			}
		}

		public Frame Frame { get => _frame; set => _frame = value; }

		private void UpdateBackButton()
		{
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
			  (_frame.CanGoBack) ? AppViewBackButtonVisibility.Visible
				: AppViewBackButtonVisibility.Collapsed;
		}
	}
}
