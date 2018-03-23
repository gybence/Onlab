using DataAccessLibrary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlabNews.Services.SettingsServices
{
	public delegate Task StatusUpdateHandler(CancellationToken token);
	public interface ISettingsService: INotifyPropertyChanged
	{		
		event StatusUpdateHandler OnUpdateStatus;
		User ActiveUser { get; set; }
		CancellationTokenSource Cts { get;}

	}
}
