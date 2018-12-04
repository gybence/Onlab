using DataAccessLibrary.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace RSSDownloader.Services
{
	public interface IRssFeedDownloader
	{
		Task<IList<SyndicationFeed>> DownloadFeedsAsync(User currentUser);
	}
}
