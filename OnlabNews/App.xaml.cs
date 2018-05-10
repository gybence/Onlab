using Prism.Unity.Windows;
using Prism.Windows.Navigation;
using System;
using System.Threading.Tasks;
using Unity;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DataAccessLibrary;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DataAccessLibrary.Model;
using OnlabNews.Services.SettingsServices;
using Unity.Lifetime;
using OnlabNews.Services.DataSourceServices;
using OnlabNews.Services.FacebookServices;
using Windows.ApplicationModel.Background;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;

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
			Window.Current.Activate();

			return Task.FromResult<object>(null);
		}


		protected override Task OnInitializeAsync(IActivatedEventArgs args)
		{
	
			Container.RegisterInstance<INavigationService>(NavigationService);
			Container.RegisterInstance<ISettingsService>(new SettingsService(), new ContainerControlledLifetimeManager());
			Container.RegisterInstance<IFacebookService>(new FacebookService(Container.Resolve<ISettingsService>()), new ContainerControlledLifetimeManager());

			Container.RegisterInstance<IArticleDataSourceService>(new ArticleDataSourceService(Container.Resolve<ISettingsService>()), new ContainerControlledLifetimeManager());
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
						db.Users.Add(new User { Name = "Default", LastLoggedIn = true });
						db.SaveChanges();
					}
					if (!db.RssFeeds.Any())
					{
						db.RssFeeds.Add(new RssFeed { Name = "444", Uri = "https://444.hu/feed" });
						db.SaveChanges();
					}
					if (!db.Subscriptions.Any())
					{
						db.Subscriptions.Add(new Subscription { UserID = 1, RssFeedID = 1 });
						db.SaveChanges();
					}
				}
				catch
				{

				}
			}
		}

		protected override void OnActivated(IActivatedEventArgs args)
		{
			base.OnActivated(args);

		}


	}
}
