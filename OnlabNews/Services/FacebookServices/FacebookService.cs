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
using System.Runtime.InteropServices;
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
		public string UserName { get; private set; }


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

			try
			{
				//if (ApplicationData.Current.LocalSettings.Values.ContainsKey(nameof(AccessToken)))
				//{
				//    AccessToken = ApplicationData.Current.LocalSettings.Values[nameof(AccessToken)] as string;
				//}
				AccessToken = new PasswordVault().Retrieve("Onlab", nameof(AccessToken)).Password; //kivetelt dob ha nem talalja meg, le koll kezelni dx

				Task.Run(() => GetUserAsync()); //TODO: ez itt nagyon nem jo
			}
			catch (COMException e) when (e.Message.Contains("Cannot get credential from Vault"))
			{
				//ha nem sikerult bejelentkezni facen erdemes a biztonsag kedveert a Default felhasznalot betolteni
				LoadUserFromDatabase("Default");
			}
		}
		
		public async Task<bool> SignInFacebookAsync()
		{
			try
			{
				var authRes = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, _loginUri, _callbackUri);
				if (authRes.ResponseStatus == WebAuthenticationStatus.Success && !IsLoggedIn)
				{
					AccessToken = new FacebookClient().ParseOAuthCallbackUrl(new Uri(authRes.ResponseData)).AccessToken;

					if (AccessToken != null) //ez arra kell ha a user az utolso resznel a cancelre nyom...
					{
						var fb = new FacebookClient(AccessToken);
						dynamic fbRes = await fb.GetTaskAsync("/me", new { fields = "email,name", access_token = AccessToken });
						UserID = fbRes["id"];
						UserName = fbRes["name"];
						//ApplicationData.Current.LocalSettings.Values[nameof(AccessToken)] = AccessToken;
						new PasswordVault().Add(new PasswordCredential("Onlab", nameof(AccessToken), AccessToken));						
						
						return true;
					}
					else
						return false;
				}
				else
					return false;
			}
			catch
			{
				return false;
			}
			
		}

		public async Task<bool> SignOutFacebookAsync()
		{
			try
			{
				var fb = new FacebookClient();
				_logoutUri = fb.GetLogoutUrl(new Dictionary<string, object>
				  {
					  { "next", "http://www.facebook.com"},
					  { "access_token", AccessToken }
				  });
				//TODO: csak akkor lehet logoutolni ha van internet whops o.o
				var res = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, _logoutUri, _callbackUri);
				new PasswordVault().Remove(new PasswordCredential("Onlab", nameof(AccessToken), AccessToken));
				AccessToken = null;
				UserID = "Default";
				return true;
			}
			catch
			{
				return false;
			}
		}

		private async Task GetUserAsync()
		{
			var fb = new FacebookClient(AccessToken);
			//var res = (JsonObject)await fb.GetTaskAsync("me?fields=email,name");
			try
			{
				dynamic res = await fb.GetTaskAsync("/me", new { fields = "email,name", access_token = AccessToken });
				var userID = (string)res["id"];
				UserID = userID;
				UserName = (string) res["name"];
				LoadUserFromDatabase(userID);
			}
			catch (WebExceptionWrapper e) when (e.Message.Contains("The server name or address could not be resolved"))
			{

			}
		}

		private void LoadUserFromDatabase(string userName)//parameterben atadni h kit kell betolteni, a biztonsag kedveert
		{
			using (var db = new AppDbContext())
			{
				var userToLoad = db.Users.Include(x => x.Subscriptions).SingleOrDefault(u => u.Name == userName);

				if (userToLoad != null && _settingsService.ActiveUser.ID != userToLoad.ID)
				{
					userToLoad.LastLoggedIn = true;
					_settingsService.ActiveUser = userToLoad;
				}
			}
		}
	}
}
