using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace OnlabNews.ViewModels
{
	public class FeedPageViewModel : ViewModelBase
	{
		#region properties

		INavigationService _navigationService;

		string _articleLink;
		public string ArticleLink { get => _articleLink; set { SetProperty(ref _articleLink, value); } }


		ObservableCollection<string> _items = new ObservableCollection<string>();
		public ObservableCollection<string> Items { get => _items; set => _items = value; }

		public DelegateCommand OnClickCommand { get; private set; }
		public DelegateCommand ArticleClickCommand { get; private set; }
		#endregion

		public FeedPageViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService;

			OnClickCommand = new DelegateCommand(async () => await OnClick());
			ArticleClickCommand = new DelegateCommand(() => ArticleClick());
		}
		public void ArticleClick()
		{
			_navigationService.Navigate("Article", ArticleLink);
			//_navigationService.Navigate("Settings",null);
		}

		public async Task OnClick()
		{
			SyndicationClient client = new SyndicationClient();
			SyndicationFeed feed;
			Uri uri = null;
			string uriString = "http://feeds.bbci.co.uk/news/rss.xml?edition=uk";
			//string uriString = "http://rss.cnn.com/rss/edition.rss";
			try
			{
				uri = new Uri(uriString);
			}
			catch (Exception e)
			{

			}
			try
			{
				client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
				feed = await client.RetrieveFeedAsync(uri);
				var title = feed.Title.Text;
				foreach (SyndicationItem item in feed.Items)
					DisplayCurrentItem(item);


				ArticleLink = Items[1];
			}
			catch
			{

			}

			//_navigationService.Navigate("Settings",null);
		}

		private void DisplayCurrentItem(SyndicationItem item)
		{

			string itemTitle = item.Title == null ? "No title" : item.Title.Text;
			string itemLink = item.Links == null ? "No link" : item.Links.FirstOrDefault().ToString();
			string itemContent = item.Content == null ? "No content" : item.Content.Text;

			string extensions = "";
			foreach (SyndicationNode node in item.ElementExtensions)
			{
				string nodeName = node.NodeName;
				string nodeNamespace = node.NodeNamespace;
				string nodeValue = node.NodeValue;
				extensions += nodeName + "\n" + nodeNamespace + "\n" + nodeValue + "\n";
			}
			//Items.Add(itemTitle + "\n" + itemLink + "\n" + itemContent + "\n" + extensions);

			Items.Add(item.Id);


		}

		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(e, viewModelState);
		}
	}
}
