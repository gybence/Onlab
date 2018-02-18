using OnlabNews.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using OnlabNews.Views;

namespace OnlabNews.Converters
{
	public class NavConverter : IValueConverter
	{
		public object Convert(object v, Type t, object p, string l)
		{
			var list = new List<object>();
			foreach (var item in (v as IEnumerable<object>))
			{
				switch (item)
				{
					case NavItemEx dto:
						list.Add(ToItem(dto));
						break;
					case NavItemHeaderEx dto:
						list.Add(ToItem(dto));
						break;
					case NavItemSeparatorEx dto:
						list.Add(ToItem(dto));
						break;
				}
			}
			return list;
		}

		NavigationViewItem ToItem(NavItemEx item)
		{
			NavigationViewItem x =  new NavigationViewItem
			{
				Content = item.Text,
				Tag = item.Tag,
				Icon =new SymbolIcon { Symbol = (Symbol)Enum.Parse(typeof(Symbol),item.Symbol)}
			};
			x.SetValue(NavProperties.PageTypeProperty, item.PageType);
			x.SetValue(NavProperties.IsStartPageProperty, item.IsStartPage);
			return x;
		}

		NavigationViewItemHeader ToItem(NavItemHeaderEx item)
		{
			return new NavigationViewItemHeader { Content = item.Text/*, Opacity = item.Opacity*/};
		}


		NavigationViewItemSeparator ToItem(NavItemSeparatorEx item)
		{
			return new NavigationViewItemSeparator { };
		}


		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
