using OnlabNews.Models;
using OnlabNews.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Services.DataSourceServices
{
    public interface IArticleDataSourceService
    {
		ObservableCollection<MutableGrouping<char, ArticleItem>> GroupedArticles { get; set; }
		Task QueryArticles();
	}
}
