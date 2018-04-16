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
using OnlabNews.Helpers;
using System.Collections.ObjectModel;
using System.Threading;

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
				UpdatePaneHeadersVisibility(this,null);
				UpdatePageHeaderContent();
			}
			get { return base.SelectedItem; }
		}

		#endregion


		public NavViewEx(){}
		public void Setup(Frame frame, INavigationService service)
		{
			//frame.Margin = new Thickness(24);
			//frame.Margin = new Thickness(24, 24, 0, 24);
			Content = _frame = frame; //641 1008
			_navigationService = service;
			_frame.Navigated += Frame_Navigated; //navigacio navview nelkul
			ItemInvoked += NavViewEx_ItemInvoked; //navigacio navview menu itemre kattintassal 
			Loaded += NavViewEx_Loaded;
			DisplayModeChanged += ChangeMarginThickness;

			RegisterPropertyChangedCallback(IsPaneOpenProperty, UpdatePaneHeadersVisibility);	
		}


		#region event subscriptions

		private void NavViewEx_Loaded(object sender, RoutedEventArgs e)
		{
			if (FindStart() is NavigationViewItem i && i != null)
			{
				_navigationService.Navigate(i.GetValue(NavProperties.PageTokenProperty).ToString(),null);
			}
			UpdateBackButton();
			UpdatePaneHeadersVisibility(this, null);
			UpdatePageHeaderContent();
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

		//We recommend 12px margins for your content area when NavigationView is in Minimal mode and 24px margins otherwise.
		private void ChangeMarginThickness(NavigationView sender, NavigationViewDisplayModeChangedEventArgs e)
		{
			if (DisplayMode == NavigationViewDisplayMode.Minimal)
				_frame.Margin = new Thickness(12, 12, 0, 12);
			else
				_frame.Margin = new Thickness(24, 24, 0, 24);
		}

		#endregion

		#region find functions

		NavigationViewItem FindStart()
			  => MenuItems.OfType<NavigationViewItem>()
				.SingleOrDefault(x => (bool)x.GetValue(NavProperties.IsStartPageProperty));


		NavigationViewItem Find(string content)
		  => MenuItems.OfType<NavigationViewItem>()
			.SingleOrDefault(x => x.Content.Equals(content));


		NavigationViewItem Find(Type type)
		  => MenuItems.OfType<NavigationViewItem>()
			.SingleOrDefault(x => type.Equals(x.GetValue(NavProperties.PageTypeProperty)));

		#endregion

		#region paneheader and back button

		public enum HeaderBehaviors { Hide, Remove, None }
		public HeaderBehaviors HeaderBehavior { get; set; } = HeaderBehaviors.Remove;
		private void UpdatePaneHeadersVisibility(DependencyObject sender, DependencyProperty dp)
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

		private void UpdateBackButton()
		{
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
			  (_navigationService.CanGoBack()) ? AppViewBackButtonVisibility.Visible
				: AppViewBackButtonVisibility.Collapsed;
		}

		#endregion

		#region pageheader content

		private static SemaphoreSlim _updatePageHeaderSemaphore = new SemaphoreSlim(1, 1);
		private void UpdatePageHeaderContent()
		{
			_updatePageHeaderSemaphore.Wait();

			try
			{
				if (_frame.Content is Page page)
				{
					if (page.GetValue(NavProperties.HeaderProperty) is string headerText && !Equals(Header, headerText))
					{
						Header = headerText;
					}


					if (!(page.GetValue(NavProperties.HeaderCommandsProperty) is ObservableCollection<object> headerCommands) || !(headerCommands.Any()))
					{
						localClearPageHeaderCommands();
					}
					else
					{
						localUpdatePageHeaderCommands(headerCommands);
					}
				}
			}
			finally
			{
				_updatePageHeaderSemaphore.Release();
			}

			#region local functions

			void localClearPageHeaderCommands()
			{
				if (!localTryGetCommandBar(out var bar))
				{
					return;
				}

				bar.PrimaryCommands.Clear();
			}

			bool localTryGetCommandBar(out CommandBar bar)
			{
				var children = XamlUtilities.RecurseChildren(this);
				var bars = children
					.OfType<CommandBar>();
				if (!bars.Any())
				{
					bar = default(CommandBar);
					return false;
				}
				bar = bars.First();
				return true;
			}

			void localUpdatePageHeaderCommands(ObservableCollection<object> headerCommands)
			{
				if (!localTryGetCommandBar(out var bar))
				{
					return;
				}

				var previous = bar.PrimaryCommands
					.OfType<DependencyObject>()
					.Where(x => x.GetValue(NavProperties.PageHeaderCommandDynamicItemProperty) is bool value && value);

				foreach (var command in previous.OfType<ICommandBarElement>().ToArray())
				{
					bar.PrimaryCommands.Remove(command);
				}

				foreach (var command in headerCommands.Reverse().OfType<DependencyObject>().ToArray())
				{
					command.SetValue(NavProperties.PageHeaderCommandDynamicItemProperty, true);
					bar.PrimaryCommands.Insert(0, command as ICommandBarElement);
				}
			}
			#endregion
		}

		#endregion


	}
}
