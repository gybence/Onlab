using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;
using System;
using System.Threading.Tasks;
using Unity;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OnlabNews
{

	sealed partial class App : PrismUnityApplication
	{
        public App()
        {
            this.InitializeComponent();
        }

		#region prism
		//protected override UIElement CreateShell(Frame rootFrame)
		//{
		//	var shell = Container.Resolve<AppShell>();
		//	shell.SetContentFrame(rootFrame);
		//	return shell;
		//}

		protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
		{
			NavigationService.Navigate("Main", null);

			//Window.Current.Activate();

			return Task.FromResult<object>(null);
		}


		protected override Task OnInitializeAsync(IActivatedEventArgs args)
		{
			Container.RegisterInstance<INavigationService>(NavigationService);

			ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
			{
				var viewModelTypeName = string.Format(System.Globalization.CultureInfo.InvariantCulture, "OnlabNews.ViewModels.{0}ViewModel, OnlabNews", viewType.Name);
				var viewModelType = Type.GetType(viewModelTypeName);
				return viewModelType;
			});

			return base.OnInitializeAsync(args);
		}



		#endregion

    }
}
