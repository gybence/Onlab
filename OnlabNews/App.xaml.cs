using Prism.Mvvm;
using Prism.Unity.Windows;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace OnlabNews
{

	sealed partial class App : PrismUnityApplication
	{
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

		#region prism
		protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
		{
			NavigationService.Navigate("Main", null);

			Window.Current.Activate();

			return Task.FromResult<object>(null);
		}


		protected override Task OnInitializeAsync(IActivatedEventArgs args)
		{
			RegisterTypes();
			return base.OnInitializeAsync(args);
		}


		private void RegisterTypes()
		{
			ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
			{
				var viewModelTypeName = string.Format(System.Globalization.CultureInfo.InvariantCulture, "OnlabNews.ViewModels.{0}ViewModel, OnlabNews", viewType.Name);
				var viewModelType = Type.GetType(viewModelTypeName);
				return viewModelType;
			});
		}
		#endregion

		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

     
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
