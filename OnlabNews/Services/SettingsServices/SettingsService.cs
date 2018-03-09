using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary;
using DataAccessLibrary.Model;

namespace OnlabNews.Services.SettingsServices
{
	public class SettingsService: ISettingsService, INotifyPropertyChanged
	{
		#region properties
	

		User _activeUser;

		public event PropertyChangedEventHandler PropertyChanged;
		public event StatusUpdateHandler OnUpdateStatus;

		public User ActiveUser {
			get => _activeUser;
			set
			{
				_activeUser = value;
				OnUpdateStatus?.Invoke();
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_activeUser)));
			}
		}

		#endregion

		public SettingsService()
		{

			using (var db = new AppDbContext())
			{
				//TODO: ne a default hanem a legutobb hasznalt legyen
				ActiveUser = db.Users.ToList().SingleOrDefault(u => u.Name.Equals("Default"));						
			}
		}


	}
}
