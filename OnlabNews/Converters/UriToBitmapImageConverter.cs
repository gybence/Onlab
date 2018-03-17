using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace OnlabNews.Converters
{
	public class UriToBitmapImageConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			BitmapImage image = new BitmapImage
			{
				UriSource = new Uri(value as string)
			};
			return image;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
