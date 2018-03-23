using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OnlabNews.Extensions
{
	public partial class NavProperties : DependencyObject
	{
		public static Type GetPageType(NavigationViewItem obj)
			=> (Type)obj.GetValue(PageTypeProperty);
		public static void SetPageType(NavigationViewItem obj, Type value)
			=> obj.SetValue(PageTypeProperty, value);
		public static readonly DependencyProperty PageTypeProperty =
			DependencyProperty.RegisterAttached("PageType", typeof(Type),
				typeof(NavProperties), new PropertyMetadata(null));


		public static bool GetIsStartPage(NavigationViewItem obj)
			=> (bool)obj.GetValue(IsStartPageProperty);
		public static void SetIsStartPage(NavigationViewItem obj, bool value)
			=> obj.SetValue(IsStartPageProperty, value);
		public static readonly DependencyProperty IsStartPageProperty =
			DependencyProperty.RegisterAttached("IsStartPage", typeof(bool),
				typeof(NavProperties), new PropertyMetadata(false));


		public static string GetPageToken(NavigationViewItem obj)
			=> (string)obj.GetValue(PageTokenProperty);
		public static void SetPageToken(NavigationViewItem obj, string value)
			=> obj.SetValue(PageTokenProperty, value);
		public static readonly DependencyProperty PageTokenProperty =
			DependencyProperty.RegisterAttached("PageToken", typeof(string),
				typeof(NavProperties), new PropertyMetadata(null));

		public static string GetHeader(Page obj)
			=> (string)obj.GetValue(HeaderProperty);
		public static void SetHeader(Page obj, string value)
			=> obj.SetValue(HeaderProperty, value);
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.RegisterAttached("Header", typeof(string),
				typeof(NavProperties), new PropertyMetadata(null));

		internal static bool GetPageHeaderCommandDynamicItem(DependencyObject obj)
		   => (bool)obj.GetValue(PageHeaderCommandDynamicItemProperty);
		internal static void SetPageHeaderCommandDynamicItem(DependencyObject obj, bool value)
			=> obj.SetValue(PageHeaderCommandDynamicItemProperty, value);
#pragma warning disable IDE1006 // Naming Styles
		internal static readonly DependencyProperty PageHeaderCommandDynamicItemProperty =
#pragma warning disable IDE1006 // Naming Styles
			DependencyProperty.RegisterAttached("PageHeaderCommandDynamicItem", typeof(bool),
				typeof(NavProperties), new PropertyMetadata(false));



		public static ObservableCollection<object> GetHeaderCommands(Page obj)
		{
			var value = (ObservableCollection<object>)obj.GetValue(HeaderCommandsProperty);
			if (value == null)
			{
				SetHeaderCommands(obj, value = new ObservableCollection<object>());
				value.CollectionChanged += (s, e) =>
				{
					// TODO
				};
			}
			return value;
		}
		public static void SetHeaderCommands(Page obj, ObservableCollection<object> value)
			=> obj.SetValue(HeaderCommandsProperty, value);
		public static readonly DependencyProperty HeaderCommandsProperty =
			DependencyProperty.RegisterAttached("HeaderCommands",
				typeof(ObservableCollection<object>),
				typeof(NavProperties), new PropertyMetadata(null));
	}
}
