using DataAccessLibrary;
using Facebook;
using Microsoft.EntityFrameworkCore;
using OnlabNews.Models;
using OnlabNews.Services.SettingsServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Credentials;

namespace OnlabNews.Services.FacebookServices
{
	public class FacebookService: IFacebookService
	{
		#region properties

		private readonly Uri _callbackUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
		private readonly Uri _loginUri;
		private Uri _logoutUri;
		private ISettingsService _settingsService;

		public string AppID { get => "2032332537092014"; }
		public string AccessToken { get; private set; }
		public bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken);
		public string UserID { get; private set; } = "Default";

		#endregion

		public FacebookService(ISettingsService settingsService)
		{
			_settingsService = settingsService;

			var fb = new FacebookClient();
			_loginUri = fb.GetLoginUrl(new
			{
				display = "popup",
				response_type = "token",
				redirect_uri = _callbackUri,
				client_id = AppID,
				scope = "email"
			});

			//if (ApplicationData.Current.LocalSettings.Values.ContainsKey(nameof(AccessToken)))
			//{
			//    AccessToken = ApplicationData.Current.LocalSettings.Values[nameof(AccessToken)] as string;
			//}

			try
			{
				AccessToken = new PasswordVault().Retrieve("Onlab", nameof(AccessToken)).Password; //kivetelt dob ha nem talalja meg, le koll kezelni dx
				Task.Run(() => GetUserAsync());
			}
			catch (Exception)
			{
				UserID = "Default";
				LoadUserFromDatabase();
			}
		}
		
		public async Task<bool> SignInFacebookAsync()
		{
			var authRes = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, _loginUri, _callbackUri);
			if (authRes.ResponseStatus == WebAuthenticationStatus.Success && !IsLoggedIn)
			{
				AccessToken = new FacebookClient().ParseOAuthCallbackUrl(new Uri(authRes.ResponseData)).AccessToken;

				if (AccessToken != null) //ez arra kell ha a user az utolso resznel a cancelre nyom...
				{
					//ApplicationData.Current.LocalSettings.Values[nameof(AccessToken)] = AccessToken;
					new PasswordVault().Add(new PasswordCredential("Onlab", nameof(AccessToken), AccessToken));

					var fb = new FacebookClient(AccessToken);
					dynamic fbRes = await fb.GetTaskAsync("/me", new { fields = "email,name", access_token = AccessToken });

					UserID = fbRes["id"];
					return true;
				}
				else
					return false;

			}
			else
				return false;
		}

		public async Task<bool> SignOutFacebookAsync()
		{
			var fb = new FacebookClient();
			_logoutUri = fb.GetLogoutUrl(new Dictionary<string, object>
				  {
					  { "next", "http://www.facebook.com"},
					  { "access_token", AccessToken }
				  });
			var res = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, _logoutUri, _callbackUri);
			if (IsLoggedIn)
			{
				new PasswordVault().Remove(new PasswordCredential("Onlab", nameof(AccessToken), AccessToken));
				AccessToken = null;
				UserID = "Default";
				return true;
			}
			else
				return false;
		}

		private async Task GetUserAsync()
		{
			var fb = new FacebookClient(AccessToken);
			//var res = (JsonObject)await fb.GetTaskAsync("me?fields=email,name");

			dynamic res = await fb.GetTaskAsync("/me", new { fields = "email,name", access_token = AccessToken });

			UserID = (string)res["id"];
			LoadUserFromDatabase();
		}

		private void LoadUserFromDatabase()
		{
			using (var db = new AppDbContext())
			{
				var userToLoad = db.Users.Include(x => x.Subscriptions).SingleOrDefault(u => u.Name == UserID);

				if (userToLoad != null)
				{
					userToLoad.LastLoggedIn = true;
					_settingsService.ActiveUser = userToLoad;
				}
			}
		}
	}
}
