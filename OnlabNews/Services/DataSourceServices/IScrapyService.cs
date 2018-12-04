using OnlabNews.Models;
using OnlabNews.Models.Scrapy;
using System.Threading.Tasks;

namespace OnlabNews.Services.DataSourceServices
{
	public interface IScrapyService
	{
		string ScrapyEndpoint { get; }
		string ScrapyBaseAddress { get; }
		string ScrapyPort { get; }
		Task<RootObject> RequestArticleScrapeAsync(ArticleItem toScrape);
	}
}
