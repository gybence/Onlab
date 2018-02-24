
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
using DataAccessLibrary;
using DataAccessLibrary.Model;
using System.Collections.ObjectModel;

namespace OnlabNews.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
		#region properties

		INavigationService _navigationService;

		string _userNameText;
		public string UserNameText { get { return _userNameText; } set { _userNameText = value; RaisePropertyChanged(); } }

		ObservableCollection<User> _users = new ObservableCollection<User>();
		public ObservableCollection<User> Users { get => _users; set => _users = value; }

		string _customText;
		public string CustomText { get => _customText; set { SetProperty(ref _customText, value); } }

		public DelegateCommand SettingsButtonOnClick { get; private set; }

		#endregion

		public SettingsPageViewModel(INavigationService navigationService) //manual reading from file example
		{
			_navigationService = navigationService;
			SettingsButtonOnClick = new DelegateCommand(() =>
			{
				_navigationService.Navigate("Main", null);
			});
			using (var db = new AppDbContext())
			{
				foreach (User u in db.Users.ToList())
					Users.Add(u);
			}
		}

		public void OnClick()
		{
			using (var db = new AppDbContext())
			{
				var x = new User { UserName = UserNameText ?? "asd" };
				db.Users.Add(x);
				db.SaveChanges();

				Users.Clear();
				foreach (User u in db.Users.ToList())
					Users.Add(u);
			}

		}

		public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
		{
			CustomText = GetType().ToString();
			base.OnNavigatedTo(e, viewModelState);

		}
	}
}
