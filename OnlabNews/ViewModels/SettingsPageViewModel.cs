using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using Windows.Web.Syndication;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OnlabNews.Views;

namespace OnlabNews.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
		#region properties

		INavigationService _navigationService;

		string _bsd;
		public string Bsd { get => _bsd; set { SetProperty(ref _bsd, value); } }

		public DelegateCommand OtherOnClick { get; private set; }

		#endregion

		public SettingsPageViewModel(INavigationService navigationService) //manual reading from file example
		{
			_navigationService = navigationService;
			OtherOnClick = new DelegateCommand(() =>
			{
				_navigationService.Navigate("Main", null);
			});
		//	XmlReaderSettings settings = new XmlReaderSettings
		//	{
		//		IgnoreWhitespace = true,
		//		IgnoreComments = true
		//	};
		//	XmlReader reader = XmlReader.Create("file.xml", settings);
		//	var feedReader = new RssFeedReader(reader);
		//	OtherOnClick = new DelegateCommand(async () =>
		//	{
		//		Bsd = "default text";
			
		//		while (await feedReader.Read())
		//		{
		//			switch (feedReader.ElementType)
		//			{
		//				// Read category
		//				case SyndicationElementType.Category:
		//					ISyndicationCategory category = await feedReader.ReadCategory();
		//					break;

		//				// Read Image
		//				case SyndicationElementType.Image:
		//					ISyndicationImage image = await feedReader.ReadImage();
		//					Bsd = image.Url.OriginalString;
		//					break;

		//				// Read Item
		//				case SyndicationElementType.Item:
		//					ISyndicationItem item = await feedReader.ReadItem();
		//					break;

		//				// Read link
		//				case SyndicationElementType.Link:
		//					ISyndicationLink link = await feedReader.ReadLink();
		//					//Bsd = link.Uri.OriginalString;
		//					break;

		//				// Read Person
		//				case SyndicationElementType.Person:
		//					ISyndicationPerson person = await feedReader.ReadPerson();
		//					break;

		//				// Read content
		//				default:
		//					ISyndicationContent content = await feedReader.ReadContent();
		//					break;
		//			}
		//		}
		//	});
		}

		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			Bsd = this.GetType().ToString();
			base.OnNavigatedTo(e, viewModelState);

		}
	}
}
