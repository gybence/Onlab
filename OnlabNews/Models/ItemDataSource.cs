using OnlabNews.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.Web.Syndication;

namespace OnlabNews.Models
{
	public class ItemDataSource
	{
		#region properties

		private static ItemDataSource instance;
		public static ItemDataSource Instance
		{
			get
			{
				if (instance == null)
					instance = new ItemDataSource();
				return instance;
			}
		}


		ObservableCollection<ArticleItem> _articleCollection = new ObservableCollection<ArticleItem>();
		public ObservableCollection<ArticleItem> ArticleCollection { get { return _articleCollection; } set { _articleCollection = value; } }

		ObservableCollection<string> _uriStrings = new ObservableCollection<string>();
		public ObservableCollection<string> UriStrings { get => _uriStrings; set => _uriStrings = value; }


		CoreDispatcher dispatcher;

		#endregion

		private ItemDataSource()
		{
			dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
			UriStrings.Add("https://index.hu/24ora/rss/");
			//UriStrings.Add("https://444.hu/feed");
			UriStrings.Add("http://rss.cnn.com/rss/edition.rss");
			//UriStrings.Add("http://feeds.bbci.co.uk/news/rss.xml?edition=uk");

			//Task.Run(QueryArticles);
		}

		public async Task QueryArticles()
		{
			try
			{
				SyndicationClient client = new SyndicationClient();
				client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
				foreach (string uriString in UriStrings)
				{
					Uri uri = new Uri(uriString);
					SyndicationFeed feed = await client.RetrieveFeedAsync(uri);
			

					foreach (SyndicationItem item in feed.Items)
						await DisplayCurrentItemAsync(item);

				}
			}
			catch
			{

			}
		}
		private async Task DisplayCurrentItemAsync(SyndicationItem item)
		{

			string itemTitle = item.Title == null ? "No title" : item.Title.Text;
			string itemLink = item.Links == null ? "No link" : item.Links.FirstOrDefault().NodeValue;
			//string itemContent = item.Content == null ? "No content" : item.Content.Text;
			
			//string extensions = "";
			//foreach (SyndicationNode node in item.ElementExtensions)
			//{
			//	string nodeName = node.NodeName;
			//	string nodeNamespace = node.NodeNamespace;
			//	string nodeValue = node.NodeValue;
			//	extensions += nodeName + "\n" + nodeNamespace + "\n" + nodeValue + "\n";
			//}
			//Items.Add(itemTitle + "\n" + itemLink + "\n" + itemContent + "\n" + extensions);
			await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
			{
				ArticleCollection.Add(new ArticleItem { Title = itemTitle, Uri = itemLink });
			});

		}
	}
}
