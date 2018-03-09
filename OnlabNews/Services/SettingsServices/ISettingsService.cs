using DataAccessLibrary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Services.SettingsServices
{
	public delegate Task StatusUpdateHandler();
	public interface ISettingsService
	{
		event PropertyChangedEventHandler PropertyChanged;
		
		event StatusUpdateHandler OnUpdateStatus;
		User ActiveUser { get; set; }
	}
}
