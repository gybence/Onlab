﻿using Microsoft.Toolkit.Uwp.Services.Facebook;
using OnlabNews.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Services.FacebookServices
{
	public interface IFacebookGraphService
	{
		string FacebookGraphAppID { get; }
		FacebookService FacebookServiceInstance { get; }
		RangeObservableCollection<FacebookPost> FacebookPosts { get; }
		void LoadFacebookPosts();
	}
}