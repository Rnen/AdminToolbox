using System;
using System.Linq;
using System.Net;
using Smod2;
using UnityEngine.Networking;

namespace AdminToolbox.API
{
	using API.Webhook;
	/// <summary>
	/// Struct used by <see cref="ATWeb.LatestRelease"/> to store the latest GitHub release info
	/// </summary>
	public struct ATReleaseInfo
	{
		/// <summary>
		/// Title of the release
		/// </summary>
		public string Title { get; }
		/// <summary>
		/// Version of the release
		/// </summary>
		public string Version { get; }
		/// <summary>
		/// GitHub Author of the release
		/// </summary>
		public string Author { get; }
		/// <summary>
		/// Download link for the release
		/// </summary>
		public string DownloadLink { get; }

		/// <summary>
		/// Constructor that takes the supplied strings from <see cref="ATWeb.GetOnlineInfo()"/>
		/// </summary>
		/// <param name="Title">Title of the release</param>
		/// <param name="Version">Version of the release</param>
		/// <param name="Author">Author of the release</param>
		/// <param name="DownloadLink">Download link of the release</param>
		internal ATReleaseInfo(string Title, string Version, string Author, string DownloadLink)
		{
			this.Title = Title;
			this.Version = Version;
			this.Author = Author;
			this.DownloadLink = DownloadLink;
		}
	}

	/// <summary>
	/// Static <see cref="AdminToolbox"/> class that contains all of the plugin's web-based methods
	/// </summary>
	public static class ATWeb
	{
		private static AdminToolbox Plugin => AdminToolbox.singleton;

		private static void Debug(string str) => Plugin.Debug("[ATWeb]: " + str);
		private static void Info(string str) => Plugin.Info("[ATWeb]: " + str);

		private const string ApiURL = "https://api.github.com/repos/Rnen/AdminToolbox/releases/latest";


		private static DateTime _lastVersionCheck = DateTime.UtcNow;
		private static ATReleaseInfo _latestReleaseInfo = new ATReleaseInfo();

		/// <summary>
		/// Returns a <see cref="ATReleaseInfo"/> class containing info about the latest GitHub release
		/// </summary>
		/// <remarks>Only updates every 5 minutes to avoid rate limits</remarks>
		public static ATReleaseInfo LatestRelease
		{
			get
			{
				if (_lastVersionCheck.AddMinutes(5) < DateTime.UtcNow || _latestReleaseInfo.Equals(default(ATReleaseInfo)))
				{
					_latestReleaseInfo = GetOnlineInfo();
					_lastVersionCheck = DateTime.UtcNow;
					Debug("Refreshed online version!");
				}
				return _latestReleaseInfo;
			}
		}

		private static ATReleaseInfo GetOnlineInfo()
		{
			Smod2.Attributes.PluginDetails Details = AdminToolbox.singleton.Details;
			if (ConfigManager.Manager.Config.GetBoolValue("atb_disable_networking", false)
				|| ConfigManager.Manager.Config.GetBoolValue("admintoolbox_disable_networking", false))
				return new ATReleaseInfo(Details.name, Details.version, Details.author, "");
			string rawResponse = string.Empty;
			string _title = "", _version = "", _author = "", _dllink = "";

			try
			{
				using (UnityWebRequest ww = UnityWebRequest.Get(ApiURL))
				{
					ww.SendWebRequest();
					DateTime timer = DateTime.UtcNow.AddSeconds(2);
					while (!ww.isDone || (!ww.downloadHandler.isDone && DateTime.UtcNow < timer)) { }
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
			catch (Exception e)
			{
				Debug("Exception: " + e.Message);
				Info(" \n\n - Downloading online version failed, skipping..." + "\n \n");
				return new ATReleaseInfo(Details.name, Details.version, Details.author, "");
			}
			return new ATReleaseInfo(_title, _version, _author, _dllink);
		}

		internal static bool NewerVersionAvailable()
		{
			if (Plugin == null) return false;
			string thisVersion = Plugin.Details.version.Split('-').FirstOrDefault().Replace(".", string.Empty);
			string onlineVersion = LatestRelease.Version.Split('-').FirstOrDefault().Replace(".", string.Empty);

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
				byte[] jsonData = Utf8Json.JsonSerializer.Serialize(discordWebHook);
				Debug("WebHook sent: \n" + Utf8.GetString(jsonData));
				return WebPost(uri, jsonData);
			}
			else
				return "Failed creating URI of WebHook link: " + url;
		}

		private static string WebPost(Uri uri, byte[] rawJsonData)
		{
			using (WebClient wb = new WebClient())
			{
				wb.Headers[HttpRequestHeader.ContentType] = "application/json";
				return Utf8Json.JsonSerializer.PrettyPrint(wb.UploadData(uri, "POST", rawJsonData));
			}
		}
	}
}
