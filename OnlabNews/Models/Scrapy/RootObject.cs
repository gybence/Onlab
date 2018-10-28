using System.Collections.Generic;

namespace OnlabNews.Models.Scrapy
{
	public class RootObject
	{
		public string Url { get; set; }
		public string ContentTitle { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string Date { get; set; }
		public List<RelatedTitle> RelatedTitles { get; set; }
		public string Lead { get; set; }
		public List<string> ArticleChildren { get; set; }
	}
}
