using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Models.Scrapy
{
	public class RootObject
	{
		public string content_title { get; set; }
		public string title { get; set; }
		public string author { get; set; }
		public string date { get; set; }
		public List<RelatedTitle> related_titles { get; set; }
		public string lead { get; set; }
		public List<string> article_children { get; set; }
	}
}
