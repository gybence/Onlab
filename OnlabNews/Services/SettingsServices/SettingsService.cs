using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary;
using DataAccessLibrary.Model;
using Microsoft.Toolkit.Uwp.Services.Facebook;
using Windows.Security.Authentication.Web;

namespace OnlabNews.Services.SettingsServices
{
	public class SettingsService: ISettingsService, INotifyPropertyChanged
	{
		#region properties

		public event PropertyChangedEventHandler PropertyChanged;
		public event StatusUpdateHandler OnUpdateStatus;

		User _activeUser;
		public User ActiveUser {
			get => _activeUser;
			set
			{
				_activeUser = value;
				OnUpdateStatus?.Invoke();
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_activeUser)));
			}
		}

		public string FacebookGraphiAppID { get => "2032332537092014"; }
	

		#endregion

		public SettingsService()
		{
			FacebookService.Instance.Initialize(FacebookGraphiAppID);
			

			//string sid = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();
			// s-1-15-2-1291395493-3554770388-778744270-2668246442-3724028671-978716057-529478274
			using (var db = new AppDbContext())
			{
				//TODO: ne a default hanem a legutobb hasznalt legyen
				ActiveUser = db.Users.ToList().SingleOrDefault(u => u.Name.Equals("Default"));						
			}
		}


	}
}
