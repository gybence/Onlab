﻿using System;
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
using Prism.Windows.Mvvm;
using OnlabNews.ViewModels;
using OnlabNews.Extensions;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnlabNews.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class FeedPage : SessionStateAwarePage
	{
		public FeedPageViewModel ViewModel
		{
			get
			{
				return DataContext as FeedPageViewModel;
			}
			private set { DataContext = value; }
		}
		public FeedPage()
		{
			this.InitializeComponent();
			this.SetValue(NavProperties.HeaderProperty, ResourceLoader.GetForCurrentView().GetString("Feeds"));
			var headerButtons = new ObservableCollection<object>();
			var refreshButton = new AppBarButton
			{
				Label = ResourceLoader.GetForCurrentView().GetString("Refresh"),
				Icon = new SymbolIcon { Symbol = (Symbol)Enum.Parse(typeof(Symbol), "Refresh") },
				Command = ViewModel.RefreshButtonCommand
			};

			headerButtons.Add(refreshButton);
			SetValue(NavProperties.HeaderCommandsProperty, headerButtons);
		}
	}
}
