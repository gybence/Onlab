﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OnlabNews.Converters
{
	public sealed class DateTimeToStringConverter : IValueConverter
	{
		public static readonly DependencyProperty FormatProperty =
			DependencyProperty.Register(nameof(Format), typeof(bool), typeof(DateTimeToStringConverter), new PropertyMetadata("G"));

		public string Format { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is DateTime dateTime && value != null)
			{
				return dateTime.ToString(Format);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return DateTime.ParseExact(value.ToString(), Format, CultureInfo.CurrentCulture);
		}
	}
}
