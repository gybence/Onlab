using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataAccessLibrary;
using DataAccessLibrary.Model;
using Microsoft.Toolkit.Uwp.Services.Facebook;
using Windows.Security.Authentication.Web;

namespace OnlabNews.Services.SettingsServices
{
	public class SettingsService: ISettingsService
	{
		#region properties

		public event PropertyChangedEventHandler PropertyChanged;
		public event StatusUpdateHandler OnUpdateStatus;

		User _activeUser;
		public User ActiveUser {
			get => _activeUser;
			set
			{
				if(_activeUser != value)
				{
					_activeUser = value;

					_cts.Cancel();
					_cts.Dispose();
					_cts = new CancellationTokenSource();

					OnUpdateStatus?.Invoke(_cts.Token);
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveUser)));
				}
			}
		}

		public CancellationTokenSource Cts { get => _cts;}

		CancellationTokenSource _cts;
	

		#endregion

		public SettingsService()
		{
			_cts = new CancellationTokenSource();
			using (var db = new AppDbContext())
			{
				ActiveUser = db.Users.ToList().SingleOrDefault(u => u.LastLoggedIn == true);						
			}
		}
	}
}
