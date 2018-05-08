using OnlabNews.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace OnlabNews.Services.FacebookServices
{
	public class FacebookGraphService: IFacebookGraphService, INotifyPropertyChanged
	{
		#region properties
		// s-1-15-2-1291395493-3554770388-778744270-2668246442-3724028671-978716057-529478274
		private readonly Uri _callbackUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
		public string FacebookGraphAppID { get => "2032332537092014"; }

		private readonly Uri _loginUri;

		public string AccessToken { get; private set; }
		public bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken);

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion


	}
}
