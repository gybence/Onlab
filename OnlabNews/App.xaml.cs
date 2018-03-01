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
using DataAccessLibrary;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DataAccessLibrary.Model;

namespace OnlabNews
{

	sealed partial class App : PrismUnityApplication
	{
        public App()
		{ 
			InitializeDataBase();
            this.InitializeComponent();			
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

		private void InitializeDataBase()
		{
			using (var db = new AppDbContext())
			{
				try
				{
					db.Database.Migrate();

					if (!db.Users.Any())
					{
						db.Users.Add(new User { Name = "Default" });
						db.SaveChanges();
					}
					if (!db.RssItems.Any())
					{
						db.RssItems.Add(new RssItem { Name = "Index", Uri = "https://index.hu/24ora/rss" });
						db.SaveChanges();
					}
					if (!db.Follows.Any())
					{
						db.Follows.Add(new Follow { UserID = 1, RssItemID = 1 });
						db.SaveChanges();
					}
				}
				catch(Exception e)
				{
					System.Diagnostics.Debug.WriteLine(e.InnerException.Message);
					System.Diagnostics.Debugger.Break();
				}
			}
		}
	}
}
