using System.Collections.Generic;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API.Extentions;
	public class PlayerListCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Lists current players to server console";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ")";

		public static readonly string[] CommandAliases = new string[] { "PLAYERS", "PLAYERLIST", "PLIST" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				Player[] players = Server.GetPlayers().ToArray();

				if (players.Length < 1) { return new string[] { "No players" }; }
				string str = players.Length + " - Players in server: \n";
				List<string> myPlayerList = new List<string>();
				foreach (Player pl in players)
				{
					myPlayerList.Add(pl.PlayerRole.RoleID + "(" + (int)pl.PlayerRole.RoleID + ")" + "  " + pl.Name + "  IP: " + pl.IPAddress + " UserID: " + pl.UserID + "\n");
				}
				myPlayerList.Sort();
				foreach (string item in myPlayerList)
				{
					str += "\n - " + item;
				}
				return new string[] { str };
			}
			else
				return deniedReply;
		}
	}
}
