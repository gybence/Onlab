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
using OnlabNews.Models;

namespace OnlabNews.ViewModels
{
	public class FeedPageViewModel : ViewModelBase
	{
		#region properties

		INavigationService _navigationService;

		ArticleItem _pickedArticle;
		public ArticleItem PickedArticle { get => _pickedArticle; set { SetProperty(ref _pickedArticle, value); } }


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
			_navigationService.Navigate("Article", PickedArticle);
			//_navigationService.Navigate("Settings",null);
		}

		public async Task OnClick()
		{
			try
			{
				SyndicationClient client = new SyndicationClient();
				client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
				string uriString = "https://index.hu/24ora/rss/";
				Uri uri = new Uri(uriString);
				SyndicationFeed feed = await client.RetrieveFeedAsync(uri);
				var title = feed.Title.Text;

				foreach (SyndicationItem item in feed.Items)
					DisplayCurrentItem(item);

				PickedArticle = ItemDataSource.Instance.ArticleCollection[1];
			}
			catch
			{

			}
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

			ItemDataSource.Instance.ArticleCollection.Add(new ArticleItem {Title=itemTitle, Uri=item.Id });


		}

		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(e, viewModelState);
		}
	}
}
