﻿using Microsoft.Toolkit.Uwp.Notifications;
using RSSDownloader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.Web.Syndication;

namespace Tasks
{
	public sealed class TileUpdaterBackgroundTask : IBackgroundTask
	{

		BackgroundTaskDeferral _deferral;
		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			_deferral = taskInstance.GetDeferral();
			try
			{
				//ha volt frissen letoltve hir akkor inkabb azt hasznaljuk majd
				var localSettings = ApplicationData.Current.LocalSettings;
				var pubDate = (string)localSettings.Values["pubDate"];

				var age = DateTime.Now - DateTime.Parse(pubDate);
				if (age.TotalHours < 1)                                             //ha kevesebb mint 1 oraja toltotte le a hireket az app akkor meg hasznalhato
				{                                                                   //mert hatha csak most kerult utemezesre a task.
					string source = localSettings.Values["source"].ToString();      //ugye legalabb 15 percet kell varni amig futhat es meg utana ki tudja mennyit
					string title = localSettings.Values["title"].ToString();

					UpdateTile(source, title);
				}
				//https://stackoverflow.com/questions/35901526/how-to-check-internet-connectivity-type-in-universal-windows-platform
				else if (NetworkInterface.GetIsNetworkAvailable())
				{

					RssFeedDownloader rfg = new RssFeedDownloader();
					var results = await rfg.DownloadFeedsAsync();

					IList<SyndicationItem> items = new List<SyndicationItem>();
					foreach (SyndicationFeed f in results)
					{
						f.Items[0].Source = f;
						items.Add(f.Items[0]);
					}
					items.OrderBy(i => i.PublishedDate);
					if (items.Count > 0)
						UpdateTile(items[0].Source.Title.Text ?? "OnlabNews", items[0].Title.Text);


				}
			}
			catch
			{

			}
			_deferral.Complete();
		}


		// https://docs.microsoft.com/en-us/windows/uwp/design/shell/tiles-and-notifications/chaseable-tile-notifications#what-to-do-with-a-chaseable-tile-notifications 
		private void UpdateTile(string source, string title)
		{
			TileUpdateManager.CreateTileUpdaterForApplication().Clear();

			TileContent content = new TileContent()
			{
				Visual = new TileVisual()
				{
					TileMedium = new TileBinding()
					{
						Branding = TileBranding.NameAndLogo,
						Content = new TileBindingContentAdaptive()
						{
							Children =
								{
									new AdaptiveText()
									{
										Text = source
									},

									new AdaptiveText()
									{
										Text = title,
										HintStyle = AdaptiveTextStyle.CaptionSubtle,
										HintWrap = true
									}
								}
						},
					},

					TileWide = new TileBinding()
					{
						Branding = TileBranding.NameAndLogo,
						Content = new TileBindingContentAdaptive()
						{
							Children =
								{
									new AdaptiveText()
									{
										Text = source,
										HintStyle = AdaptiveTextStyle.Subtitle

									},

									new AdaptiveText()
									{
										Text = title,
										HintStyle = AdaptiveTextStyle.CaptionSubtle,
										HintWrap = true
									}
								}
						},
					}
				}
			};

			var notification = new TileNotification(content.GetXml());
			notification.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(5);
			TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

		}
	}
}