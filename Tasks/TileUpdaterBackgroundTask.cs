using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace Tasks
{
	public sealed class TileUpdaterBackgroundTask: IBackgroundTask
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			var vmi = (taskInstance.TriggerDetails as ApplicationTriggerDetails).Arguments;
			//taskInstance.Canceled += OnCanceled;
			var nemtom = vmi.Values.ToList();
			UpdateTile(nemtom[0].ToString(), nemtom[1].ToString());
		}


		// https://docs.microsoft.com/en-us/windows/uwp/design/shell/tiles-and-notifications/chaseable-tile-notifications#what-to-do-with-a-chaseable-tile-notifications 
		private void UpdateTile(string title, string source)
		{
			//if (GroupedArticles.Count != 0)
			{
				
				// Construct the tile content
				TileContent content = new TileContent()
				{
					Visual = new TileVisual()
					{
						TileMedium = new TileBinding()
						{
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
									},

									//new AdaptiveText()
									//{
									//	Text = body,
									//	HintStyle = AdaptiveTextStyle.CaptionSubtle
									//}
								}
							},
						},

						TileWide = new TileBinding()
						{
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
									},

									//new AdaptiveText()
									//{
									//	Text = body,
									//	HintStyle = AdaptiveTextStyle.CaptionSubtle
									//}
								}
							},
						}
					}
				};


				// Then create the tile notification
				var notification = new TileNotification(content.GetXml());
				notification.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(1);

				// And send the notification
				TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
			}
		}
	}
}
