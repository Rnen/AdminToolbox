using System;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;
using SMItem = Smod2.API.Item;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class PlayerCommand : ICommandHandler
	{
		private const int LeftPadding = 3;

		private Server Server => PluginManager.Manager.Server;
		public string GetCommandDescription() => "Gets toolbox info about spesific player";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYERNAME/ID/UserId]";

		public static readonly string[] CommandAliases = new string[] { "PLAYER", "P", "PLAYERINFO", "PINFO" };

		private static string StringToMax(string text, int max = 32)
		{
			while (text.Length < max)
				text += ' ';
			return text;
		}

		private string BuildTwoLiner(string str1, string str2 = "")
		{
			if (!string.IsNullOrEmpty(str2))
				return StringToMax(str1) + "|" + StringToMax(str2.PadLeft(LeftPadding));
			else
				return StringToMax(str1);
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (Server.GetPlayers().Count > 0)
				{
					Player myPlayer = (args.Length > 0) ? GetPlayerFromString.GetPlayer(args[0]) : null;
					if (myPlayer == null && sender is Player sendingPlayer)
						myPlayer = sendingPlayer;
					else if (myPlayer == null)
						if (args.Length > 0)
							return new string[] { "Could not find player: " + args[0] };
						else
							return new string[] { GetUsage() };

					//Handling player stats
					AdminToolbox.AddMissingPlayerVariables(myPlayer);
					AdminToolbox.atfileManager.PlayerStatsFileManager(myPlayer.UserId, Managers.ATFile.PlayerFile.Write);
					PlayerSettings playerDict = AdminToolbox.ATPlayerDict.TryGetValue(myPlayer.UserId, out PlayerSettings ps) ? ps : new PlayerSettings(myPlayer.UserId);

					//Inventory
					string playerInv = string.Empty;
					foreach (SMItem i in myPlayer.GetInventory().Where(i => i.ItemType != Smod2.API.ItemType.NULL))
						playerInv += i.ItemType + ", ";
					if (playerInv == string.Empty) playerInv = "Empty Inventory";

					//Calculating remaining jail time
					int remainingJailTime = ((int)playerDict.JailedToTime.Subtract(DateTime.Now).TotalSeconds >= 0) ? (int)playerDict.JailedToTime.Subtract(DateTime.Now).TotalSeconds : 0;

					string _playerRole = sender.IsPlayer() ? myPlayer.ToColoredRichTextRole() : Smod2.API.RoleType.UNASSIGNED + "";
					string _roleColor = myPlayer.GetUserGroup().Color ?? "default";
					string _serverRole = myPlayer.GetRankName() ?? "";

					//Building string
					string playerInfoString = Environment.NewLine + Environment.NewLine +
							"Player: (" + myPlayer.PlayerId + ") " + myPlayer.Name + Environment.NewLine +
						BuildTwoLiner(" - UserId: " + myPlayer.UserId, " - IP: " + myPlayer.IpAddress.Replace("::ffff:", string.Empty)) + Environment.NewLine +
						BuildTwoLiner(" - Server Rank: " + "<color=" + _roleColor + ">" + _serverRole + "</color>") + Environment.NewLine +
						BuildTwoLiner(" - Role: " + _playerRole, " - Health: " + myPlayer.HP) + Environment.NewLine +
						BuildTwoLiner(" - AdminToolbox Toggables: ") + Environment.NewLine +
						BuildTwoLiner("   - Godmode: " + playerDict.godMode, " - NoDmg: " + playerDict.dmgOff) + Environment.NewLine +
						BuildTwoLiner("   - OverwatchMode: " + myPlayer.OverwatchMode, " - KeepSettings: " + playerDict.keepSettings) + Environment.NewLine +
						BuildTwoLiner("   - BreakDoors: " + playerDict.destroyDoor, " - PlayerLockDown: " + playerDict.lockDown) + Environment.NewLine +
						BuildTwoLiner("   - LockDoors: " + playerDict.lockDoors) + Environment.NewLine +
						BuildTwoLiner("   - InstantKill: " + playerDict.instantKill, " - GhostMode: " + myPlayer.GetGhostMode()) + Environment.NewLine +
						BuildTwoLiner("   - IsJailed: " + playerDict.isJailed, " - Released In: " + remainingJailTime) + Environment.NewLine +
						BuildTwoLiner(" - Stats:") + Environment.NewLine +
						BuildTwoLiner("   - Kills: " + playerDict.PlayerStats.Kills, " - TeamKills: " + playerDict.PlayerStats.TeamKills) + Environment.NewLine +
						BuildTwoLiner("   - Deaths: " + playerDict.PlayerStats.Deaths, " - Times Banned: " + playerDict.PlayerStats.BanCount) + Environment.NewLine +
						BuildTwoLiner("   - Playtime: " + (int)playerDict.PlayerStats.MinutesPlayed + " minutes", " - Rounds Played: " + playerDict.PlayerStats.RoundsPlayed) + Environment.NewLine +
						BuildTwoLiner(" - Position:") + Environment.NewLine +
						BuildTwoLiner("  - X:" + (int)myPlayer.GetPosition().x + " Y:" + (int)myPlayer.GetPosition().y + " Z:" + (int)myPlayer.GetPosition().z) + Environment.NewLine +
						BuildTwoLiner(" - Inventory: " + playerInv) + Environment.NewLine;
					if (sender.IsPlayer())
						return new string[] { playerInfoString.Replace("True ", "<color=green>" + "True" + " </color>").Replace("False", "<color=red>" + "False" + "</color>") };
					else
						return new string[] { playerInfoString };
				}
				else
					return new string[] { "Server is empty!" };
			}
			else
				return deniedReply;
		}
	}
}
