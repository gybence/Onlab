using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary;
using DataAccessLibrary.Model;
using Microsoft.EntityFrameworkCore;

namespace OnlabNews.Models
{
	public class Settings
	{
		#region properties

		private static Settings instance;
		public static Settings Instance
		{
			get
			{
				if (instance == null)
					instance = new Settings();
				return instance;
			}
		}

		User activeUser;
		public User ActiveUser { get => activeUser; set => activeUser = value; }

		#endregion

		private Settings()
		{
			using (var db = new AppDbContext())
			{
				//TODO: ne a default hanem a legutobb hasznalt legyen
				ActiveUser = db.Users.ToList().SingleOrDefault(u => u.Name.Equals("Default"));						
			}
		}


	}
}
