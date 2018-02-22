using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Models
{
	public class RssItem
	{
		public int RssItemID { get; set; }
		public string Name { get; set; }
		public string Uri { get; set; }

		public int UserID { get; set; }
		public User User { get; set; }

		//public ICollection<User> Users { get; set; }
	}
}
