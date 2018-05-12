using OnlabNews.Extensions;
using OnlabNews.ViewModels;
using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnlabNews.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ArticlePage : SessionStateAwarePage
	{
		public ArticlePageViewModel ViewModel
		{
			get
			{
				return DataContext as ArticlePageViewModel;
			}
			private set { DataContext = value; }
		}
		public ArticlePage()
		{
			this.InitializeComponent();

			this.SetValue(NavProperties.HeaderProperty, ResourceLoader.GetForCurrentView().GetString("Article"));

			var headerButtons = new ObservableCollection<object>();
			var shareButton = new AppBarButton
			{
				Label = ResourceLoader.GetForCurrentView().GetString("Share"),
				Icon = new SymbolIcon { Symbol = (Symbol)Enum.Parse(typeof(Symbol), "Share") },
				Command = ViewModel.ShareButtonCommand
			};
	
			headerButtons.Add(shareButton);
			SetValue(NavProperties.HeaderCommandsProperty, headerButtons);

			this.Unloaded += (sender, e) =>
			{
				ViewModel = null;
			};
		}
	}
}
