using Microsoft.Toolkit.Uwp.Services.Facebook;
using OnlabNews.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Services.FacebookServices
{
	public class FacebookGraphService: IFacebookGraphService, INotifyPropertyChanged
	{
		#region properties
		public string FacebookGraphAppID { get => "2032332537092014"; }

		public event PropertyChangedEventHandler PropertyChanged;

		public FacebookService FacebookServiceInstance { get => FacebookService.Instance; }

		RangeObservableCollection<FacebookPost> _fbCollection = new RangeObservableCollection<FacebookPost>();
		public RangeObservableCollection<FacebookPost> FacebookPosts { get { return _fbCollection; } set { _fbCollection = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FacebookPosts))); } }

		#endregion

		public FacebookGraphService()
		{
			//string sid = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();
			// s-1-15-2-1291395493-3554770388-778744270-2668246442-3724028671-978716057-529478274
			FacebookService.Instance.Initialize(FacebookGraphAppID);
			//LoadFaceBookInfo();
		}

		public async void LoadFacebookPosts()
		{
			if (FacebookService.Instance.LoggedUser != null)
			{
				var items = await FacebookService.Instance.RequestAsync(FacebookDataConfig.MyFeed, 10);
				FacebookPosts.AddRange(items);
			}
		}
	}
}
