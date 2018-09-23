using DataAccessLibrary;
using DataAccessLibrary.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace RSSDownloader
{
	public sealed class RssFeedDownloader
	{

		public async Task<IList<SyndicationFeed>> DownloadFeedsAsync(User currentUser = null)
		{
			var results = new List<SyndicationFeed>();
			using (var db = new AppDbContext())
			{
				if (currentUser == null)
					currentUser = db.Users.SingleOrDefault(u => u.LastLoggedIn == true);

				SyndicationClient client = new SyndicationClient();
				client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

				foreach (Subscription f in db.Subscriptions.Where(u => u.UserID == currentUser.ID)
														   .Include(r => r.RssFeed).ToList())
				{
					try
					{
						Uri uri = new Uri(f.RssFeed.Uri);
						//TODO: timeoutot lekezelni
						SyndicationFeed feed = await client.RetrieveFeedAsync(uri);
						feed.Items.OrderBy(i => i.PublishedDate);
						results.Add(feed);
					}
					catch (COMException e) when (RssExceptionFilter(e) == true)
					{
						//lehet nincs internet, vagy a DNS szerver rip
					}
					catch (UriFormatException e)
					{

					}

				}
			}
			return results;
		}

		private bool RssExceptionFilter(Exception e)
		{
			return (e.Message.Contains("The server name or address could not be resolved") || e.Message.Contains("Invalid XML") || e.Message.Contains("404"));
		}
	}
}
