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
using OnlabNews.Extensions;
using Windows.ApplicationModel.Resources;
using OnlabNews.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnlabNews.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : SessionStateAwarePage
    {
		public SettingsPageViewModel ViewModel
		{
			get
			{
				return DataContext as SettingsPageViewModel;
			}
			private set
			{
				DataContext = value;
			}
		}
		public SettingsPage()
        {
			
            this.InitializeComponent();
			this.SetValue(NavProperties.HeaderProperty, ResourceLoader.GetForCurrentView().GetString("Settings"));
		}
	}
}
