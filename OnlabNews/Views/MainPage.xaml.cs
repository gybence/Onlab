﻿using OnlabNews.ViewModels;
using Prism.Windows.Mvvm;
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
using DataAccessLibrary;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnlabNews.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : SessionStateAwarePage
	{
		public MainPageViewModel ViewModel
		{
			get
			{
				return DataContext as MainPageViewModel;
			}
		}
		public MainPage()
		{
			this.InitializeComponent();
		}

		private void AddData(object sender, RoutedEventArgs e)
		{
			DataAccess.AddData(Input_Box.Text);

			Output.ItemsSource = DataAccess.GetData();
		}
	}
}
