using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Extensions
{

	public class NavItemEx
	{
		public string Symbol { get; set; }
		public string Content { get; set; }
		public string PageToken { get; set; }
		public Type PageType { get; set; }
		public bool IsStartPage { get; set; } = false;
	}
	public class NavItemHeaderEx 
	{
		public string Text { get; set; }
	}
	public class NavItemSeparatorEx { }
}
