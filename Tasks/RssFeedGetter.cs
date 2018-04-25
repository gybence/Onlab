using DataAccessLibrary;
using DataAccessLibrary.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Syndication;

namespace Tasks
{
	public sealed class RssFeedGetter
	{
		IList<SyndicationFeed> _result = new List<SyndicationFeed>();

		public IList<SyndicationFeed> Result { get => _result; set => _result = value; }

		public IAsyncAction DownloadFeedsAsync()
		{
			return Task.Run(async () =>
			{
				using (var db = new AppDbContext())
				{
					SyndicationClient client = new SyndicationClient();
					client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
					var currentUser = db.Users.SingleOrDefault(u => u.LastLoggedIn == true);
					foreach (Subscription f in db.Subscriptions.Where(u => u.UserID == currentUser.ID).Include(r => r.RssFeed).ToList())
					{
						//s
						Uri uri = new Uri(f.RssFeed.Uri);
						//TODO: timeoutot lekezelni
						SyndicationFeed feed = await client.RetrieveFeedAsync(uri);
						feed.Items.OrderBy(i => i.PublishedDate);
						Result.Add(feed);
					}
				}
			}).AsAsyncAction();
		}
	}
}
