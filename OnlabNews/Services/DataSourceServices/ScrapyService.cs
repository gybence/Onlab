using Newtonsoft.Json;
using OnlabNews.Models;
using OnlabNews.Models.Scrapy;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Services.DataSourceServices
{
	public class ScrapyService : IScrapyService
	{

		#region properties

		public string ScrapyBaseAddress => "http://104.40.246.35";
		public string ScrapyPort => "5000";
		public string ScrapyEndpoint => "/scrape";

		#endregion

		public async Task<RootObject> RequestArticleScrapeAsync(ArticleItem toScrape)
		{
			using (HttpClient httpClient = new HttpClient())
			{
				string address = ScrapyBaseAddress + ":" + ScrapyPort;
				httpClient.BaseAddress = new Uri(address);
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));

				string endpoint = ScrapyEndpoint;

				try
				{
					var content = new StringContent("{\"url\":\"" + toScrape.Uri + "\"}", Encoding.UTF8, "application/json");
					HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

					if (response.IsSuccessStatusCode)
					{
						string jsonResponse = await response.Content.ReadAsStringAsync();
						return JsonConvert.DeserializeObject<RootObject>(jsonResponse);
					}
					else
					{
						return null;
					}
				}
				catch (Exception)
				{
					return null;
				}
			}
		}
	}
}
