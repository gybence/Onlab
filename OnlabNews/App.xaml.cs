﻿using OnlabNews.Views;
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
using OnlabNews.ViewModels;
using OnlabNews.Services.SettingsServices;
using Unity.Lifetime;
using OnlabNews.Services.DataSourceServices;

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
			Container.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager());
			Container.RegisterType<IArticleDataSourceService, ArticleDataSource>(new ContainerControlledLifetimeManager());

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
					if (!db.RssFeeds.Any())
					{
						db.RssFeeds.Add(new RssFeed { Name = "Index", Uri = "https://index.hu/24ora/rss" });
						db.SaveChanges();
					}
					if (!db.Subscriptions.Any())
					{
						db.Subscriptions.Add(new Subscription { UserID = 1, RssFeedID = 1 });
						db.SaveChanges();
					}
				}
				catch(Exception e)
				{
					System.Diagnostics.Debugger.Break();
				}
			}
		}
	}
}
