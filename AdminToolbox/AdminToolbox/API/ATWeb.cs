using System;
using System.Linq;
using System.Net;
using Smod2;
using UnityEngine.Networking;

namespace AdminToolbox.API
{
	using API.Webhook;
	/// <summary>
	/// Static <see cref="AdminToolbox"/> class that contains all of the plugins web-based methods
	/// </summary>
	public static class ATWeb
	{
		private static AdminToolbox Plugin => AdminToolbox.plugin;

		/// <summary>
		/// Class for storing the latest GitHub release info
		/// </summary>
		public class AT_LatestReleaseInfo
		{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
			public string Title { get; }
			public string Version { get; }
			public string Author { get; }
			public string DownloadLink { get; }

			public AT_LatestReleaseInfo(string Title, string Version, string Author, string DownloadLink)
			{
				this.Title = Title;
				this.Version = Version;
				this.Author = Author;
				this.DownloadLink = DownloadLink;
			}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
		}

		/// <summary>
		/// Returns a <see cref="AT_LatestReleaseInfo"/> class containing info about the latest GitHub release
		/// </summary>
		public static AT_LatestReleaseInfo GetOnlineInfo(AdminToolbox plugin)
		{
			if (ConfigManager.Manager.Config.GetBoolValue("atb_disable_networking", false)
				|| ConfigManager.Manager.Config.GetBoolValue("admintoolbox_disable_networking", false)) return new AT_LatestReleaseInfo(plugin.Details.name, plugin.Details.version, plugin.Details.author, "");
			string rawResponse = string.Empty;
			string apiURL = "https://api.github.com/repos/Rnen/AdminToolbox/releases/latest";
			string _title = "", _version = "", _author = "", _dllink = "";

			try
			{
				using (UnityWebRequest ww = UnityWebRequest.Get(apiURL))
				{
					ww.SendWebRequest();
					DateTime timer = DateTime.Now.AddSeconds(2);
					while (!ww.isDone || (!ww.downloadHandler.isDone && DateTime.Now < timer)) { }
					rawResponse = ww.downloadHandler.text;
					if (string.IsNullOrEmpty(rawResponse))
						throw new Exception("[AdminToolbox]: GitHub web request response was NullOrEmpty!");
					string FindValue(string key)
					{
						//plugin.Debug("Searched: " + key);
						string str = rawResponse.Split(Environment.NewLine.ToCharArray()).Where(s => s.Trim().StartsWith("\"" + key)).FirstOrDefault().Split(new[] { ':' }, 2).Last().Replace("\"", string.Empty).Trim(',').Trim();
						//plugin.Debug("Found: " + str);
						return str;
					}
					_title = FindValue("name");
					_version = FindValue("tag_name");
					_author = FindValue("login");
					_dllink = FindValue("html_url"); //FindValue("browser_download_url");
					if (string.IsNullOrEmpty(_version))
						throw new Exception("[AdminToolbox]: GitHub version was NullOrEmpty!");
				}
			}
			catch
			{
				plugin.Info(" \n\n - Downloading online version failed, skipping..." + "\n \n");
				return new AT_LatestReleaseInfo(plugin.Details.name, plugin.Details.version, plugin.Details.author, "");
			}
			return new AT_LatestReleaseInfo(_title, _version, _author, _dllink);
		}

		internal static bool NewerVersionAvailable()
		{
			if (Plugin == null) return false;
			string thisVersion = Plugin.Details.version.Split('-').FirstOrDefault().Replace(".", string.Empty);
			string onlineVersion = Plugin.GetGitReleaseInfo().Version.Split('-').FirstOrDefault().Replace(".", string.Empty);

			if (int.TryParse(thisVersion, out int thisV)
				&& int.TryParse(onlineVersion, out int onlineV)
				&& onlineV > thisV)
				return true;
			else return false;
		}

		internal static string SendWebhook(DiscordWebhook discordWebHook, string url)
		{
			if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
			{
				string jsonData = UnityEngine.JsonUtility.ToJson(discordWebHook, true);

				return WebPost(uri, jsonData);
			}
			else
				return "Failed creating URI of WebHook link: " + url;
		}

		private static string WebPost(Uri uri, string rawJsonData)
		{
			using (WebClient wb = new WebClient())
			{
				wb.Headers[HttpRequestHeader.ContentType] = "application/json";
				return wb.UploadString(uri, "POST", rawJsonData);
			}
		}
	}
}
