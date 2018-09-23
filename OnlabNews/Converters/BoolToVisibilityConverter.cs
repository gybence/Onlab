using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OnlabNews.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (parameter?.ToString() == "invert")
			{
				return (bool)value == true ? Visibility.Collapsed : Visibility.Visible;
			}
			else
			{
				return (bool)value == true ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (parameter?.ToString() == "invert")
			{
				return (Visibility)value == Visibility.Collapsed;
			}
			else
			{
				return (Visibility)value == Visibility.Visible;
			}
		}
	}
}
