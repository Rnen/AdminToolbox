using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Networking;


namespace AdminToolbox.API
{
	/// <summary>
	/// Static <see cref="AdminToolbox"/> class that contains all of the plugins web-based methods
	/// </summary>
	public static class ATWeb
	{
		/// <summary>
		/// Class for storing the latest GitHub release info
		/// </summary>
		public class AT_LatestReleaseInfo
		{
			readonly string title;
			readonly string version;
			readonly string author;
			readonly string downloadLink;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
			public string Title => title;
			public string Version => version;
			public string Author => author;
			public string DownloadLink => downloadLink;

			public AT_LatestReleaseInfo(string Title, string Version, string Author, string DownloadLink)
			{
				this.title = Title;
				this.version = Version;
				this.author = Author;
				this.downloadLink = DownloadLink;
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
			string _title = "", _version = "", _author = "", _dllink ="";
		
			try
			{
				using (UnityWebRequest ww = UnityWebRequest.Get(apiURL))
				{
					ww.SendWebRequest();
					DateTime timer = DateTime.Now.AddSeconds(2);
					while (!ww.isDone || !ww.downloadHandler.isDone && DateTime.Now < timer) { }
					rawResponse = ww.downloadHandler.text;
					if (string.IsNullOrEmpty(rawResponse))
						throw new Exception();
					string FindValue(string key)
					{
						plugin.Debug("Searched: " + key);
						string str = rawResponse.Split(Environment.NewLine.ToCharArray()).Where(s => s.Trim().StartsWith("\"" + key)).FirstOrDefault().Split(new[] { ':' }, 2).Last().Replace("\"", string.Empty).Trim(',').Trim();
						plugin.Debug("Found: " + str);
						if (key.ToLower() == "tag_name") return str.Split('-').FirstOrDefault();
						return str;
					}
					_title = FindValue("name");
					_version = FindValue("tag_name");
					_author = FindValue("login");
					_dllink = FindValue("html_url"); //FindValue("browser_download_url");
					if (string.IsNullOrEmpty(_version))
						throw new Exception();
				}
			}
			catch
			{
				plugin.Info(" \n\n - Downloading online version failed, skipping..." + "\n \n");
				return new AT_LatestReleaseInfo(plugin.Details.name, plugin.Details.version, plugin.Details.author, "");
			}
			return new AT_LatestReleaseInfo(_title,_version,_author,_dllink);
		}
	}
}
