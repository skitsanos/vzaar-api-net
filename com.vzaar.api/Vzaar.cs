using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;

namespace com.vzaar.api
{
	public class Vzaar
	{
		public string username;
		public string token;

		public bool enableFlashSupport = false;

		public string apiUrl = "https://vzaar.com";

		public Vzaar()
		{
		}

		public Vzaar(string username, string token)
		{
			this.username = username;
			this.token = token;
		}

		public string whoAmI()
		{
			var url = apiUrl + "/api/test/whoami.json";

			var response = executeRequest(url);
			var jo = (JObject)JsonConvert.DeserializeObject(response);
			var username = (String)jo["vzaar_api"]["test"]["login"];
			return username;
		}

		/// <summary>
		/// This API call returns the user's public details along with it's relevant metadata.
		/// </summary>
		/// <param name="username">is the vzaar login name for the user. Note: This must be the actual username and not the email address</param>
		/// <returns></returns>
		public UserDetails getUserDetails(string username)
		{
			var url = apiUrl + "/users/" + username + ".json";
			var response = executeRequest(url);
			var jo = (JObject)JsonConvert.DeserializeObject(response);

			var details = new UserDetails
			{
				videoCount = (Int64)jo["video_count"],
				videosTotalSize = (Int64)jo["video_total_size"],
				maxFileSize = (Int64)jo["max_file_size"],
				playCount = (Int64)jo["play_count"],
				version = (string)jo["version"],
				authorId = (int)jo["author_id"],
				authorAccountTitle = (string)jo["author_account_title"],
				authorName = (string)jo["author_name"],
				authorAccount = (int)jo["author_account"],
				authorUrl = (string)jo["author_url"],
				createdAt = DateTime.Parse((string)jo["created_at"]),
				bandwidthThisMonth = (Int64)jo["bandwidth_this_month"]
			};

			var bandwidth = new List<UserBandwidthDetails>();
			JsonConvert.PopulateObject(jo["bandwidth"].ToString(), bandwidth);
			details.bandwidth = bandwidth;

			return details;
		}

		/// <summary>
		/// This API call returns the details and rights for each vzaar subscription account type along with it's relevant metadata. This will show the details of the packages available here: http://vzaar.com/pricing
		/// </summary>
		/// <param name="accountId">is the vzaar account type. This is an integer.</param>
		/// <returns></returns>
		public AccountDetails getAccountDetails(int accountId)
		{
			var url = apiUrl + "/api/accounts/" + accountId + ".json";

			var response = executeRequest(url);
			var jo = (JObject)JsonConvert.DeserializeObject(response);

			var details = new AccountDetails
			{
				accountId = (int)jo["account_id"],
				title = (string)jo["title"],
				bandwidth = (Int64)jo["bandwidth"],
				cost = new AccountCostDetails
				{
					monthly = (int)jo["cost"]["monthly"],
					currency = (string)jo["cost"]["currency"]
				},
				rights = new AccountRightsDetails
				{
					borderless = (bool)jo["rights"]["borderless"],
					searchEnhancer = (bool)jo["rights"]["searchEnhancer"]
				}
			};
			return details;
		}

		/// <summary>
		/// vzaar uses the oEmbed open standard for allowing 3rd parties to integrated with the vzaar. 
		/// </summary>
		/// <param name="videoId"></param>
		/// <returns></returns>
		public VideoDetails getVideoDetails(int videoId)
		{
			var url = apiUrl + "/api/videos/" + videoId + ".json";

			var response = executeRequest(url);
			var jo = (JObject)JsonConvert.DeserializeObject(response);

			var result = new VideoDetails
			{
				duration = (decimal)jo["duration"],
				type = (string)jo["type"],
				height = (int)jo["height"],
				width = (int)jo["width"],
				url = (string)jo["video_url"],
				provider = new VideoDetailsProvider
				{
					name = (string)jo["provider_name"],
					url = (string)jo["provider_url"]
				},
				playCount = (Int64)jo["play_count"],
				videoStatus = new VideoDetailsVideoStatus
				{
					id = (int)jo["video_status_id"],
					description = (string)jo["video_status_description"]
				},
				thumbnail = new VideoDetailsThumbnail
				{
					height = (int)jo["thumbnail_height"],
					width = (int)jo["thumbnail_width"],
					url = (string)jo["thumbnail_url"]
				},
				author = new VideoDetailsAuthor
				{
					name = (string)jo["author_name"],
					url = (string)jo["author_url"]
				},
				poster = "http://view.vzaar.com/" + videoId + "/image",
				html = (string)jo["html"],
				framegrab = new VideoDetailsFramegrab
				{
					height = (int)jo["framegrab_height"],
					width = (int)jo["framegrab_width"],
					url = (string)jo["framegrab_url"]
				},
				totalSize = (Int64)jo["total_size"],
				title = (string)jo["title"],
				description = (string)jo["description"]
			};
			return result;
		}

		/// <summary>
		/// This API call returns a list of the user's active videos along with it's relevant metadata. 20 videos are returned by default but this is customisable.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public List<Video> getVideoList(VideoListQuery query)
		{
			var url = apiUrl + "/api/" + username + "/videos.json?count=" + query.count.ToString();

			if (query.labels.Length > 0)
			{
				url += "&labels=" + String.Join(",", query.labels); //should we have it URL Encoded?
			}

			if (query.status != String.Empty)
			{
				url += "&status=" + query.status;
			}

			if (query.sort == VideoListSorting.ASCENDING)
			{
				url += "&sort=" + "asc";
			}
			else
			{
				url += "&sort=" + "desc";
			}

			if (query.title != String.Empty)
			{
				url += "&title=" + HttpUtility.UrlEncode(query.title);
			}

			var response = executeRequest(url);
			var jo = (JArray)JsonConvert.DeserializeObject(response);

			var result = new List<Video>();

			foreach (var o in jo)
			{
				var video = new Video
				{
					status = (string)o["status"],
					statusId = (int)o["status_id"],
					duration = (decimal)o["duration"],
					height = (int)o["height"],
					width = (int)o["width"],
					playCount = (Int64)o["play_count"],
					version = (string)o["version"],
					thumbnail = (string)o["thumbnail"],
					url = (string)o["url"],
					id = (Int64)o["id"],
					title = (string)o["title"],
					user = new VideoAuthor
					{
						videoCount = (Int64)o["user"]["video_count"],
						name = (string)o["user"]["author_name"],
						account = (int)o["user"]["author_account"],
						url = (string)o["user"]["author_url"]
					}
				};

				result.Add(video);
			}

			return result;
		}
		///

		public string getUploadSignature()
		{
			var url = apiUrl + "/api/videos/signature";

			if (enableFlashSupport)
			{
				apiUrl += "?=flash_request=true";
			}

			var response = executeRequest(url);
			return response;
		}

		public string getUploadSignature(string redirectUrl)
		{
			var url = apiUrl + "/api/videos/signature";

			if (enableFlashSupport)
			{
				apiUrl += "?=flash_request=true";
			}

			if (redirectUrl != String.Empty)
			{
				if (!enableFlashSupport)
				{
					url += "&success_action_redirect=" + redirectUrl;
				}
				else
				{
					url += "?success_action_redirect=" + redirectUrl;
				}
			}

			var response = executeRequest(url);
			return response;
		}

		public void uploadVideo(string path)
		{
		}

		public void processVideo()
		{
		}

		public void deleteVideo(Int64 videoId)
		{
		}

		public void editVideo()
		{
		}

		///

		private string executeRequest(string url)
		{
			return executeRequest(url, "GET", null);
		}

		private string executeRequest(string url, string method, string data)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = method;

			//sign the request
			var consumer = new OAuth.OAuthConsumer();
			consumer.SetTokenWithSecret(username, token);
			request = consumer.Sign(request);

			switch (method.ToUpper())
			{
				case "GET":
					break;

				default:
					throw new Exception("HTTP Method " + method + " is not supported");
			}

			var response = request.GetResponse();
			Debug.WriteLine(((HttpWebResponse)response).StatusDescription);

			var reader = new StreamReader(response.GetResponseStream());
			var rawResponse = reader.ReadToEnd();

			return rawResponse;
		}
	}


}
