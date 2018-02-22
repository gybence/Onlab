using OnlabNews.Views;
using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;
using System;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;
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
			//using (var db = new AppContext())
			//{
			//	try { db.Database.Migrate(); }
			//	catch (Exception ex)
			//	{
			//		// do something
			//	}
			//}
		}

		#region prism
		protected override UIElement CreateShell(Frame rootFrame)
		{
			var shell = Container.Resolve<AppShell>();
			var service = Container.Resolve<INavigationService>();
			shell.SetContentFrame(rootFrame, service);
			return shell;
		}



		protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
		{
			//NavigationService.Navigate("Main", null);

			Window.Current.Activate();

			return Task.FromResult<object>(null);
		}


		protected override Task OnInitializeAsync(IActivatedEventArgs args)
		{
			Container.RegisterInstance<INavigationService>(NavigationService);
			//Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));

			return base.OnInitializeAsync(args);
		}



		#endregion

    }
}
