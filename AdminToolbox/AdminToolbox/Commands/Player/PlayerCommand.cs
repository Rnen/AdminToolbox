using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.Command
{
	class PlayerCommand : ICommandHandler
	{
		const int LeftPadding = 3;
		Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Gets toolbox info about spesific player";

		public string GetUsage() => "(P / PLAYER) [PLAYERNAME/ID/STEAMID]";

		private static string StringToMax(string text, int max = 32)
		{
			while (text.Length < max)
				text += ' ';
			return text;
		}
		string BuildTwoLiner(string str1, string str2 = "")
		{
			if(!string.IsNullOrEmpty(str2))
				return StringToMax(str1) + "|" + StringToMax(str2.PadLeft(LeftPadding));
			else
				return StringToMax(str1);
		}

		bool IsPlayer(ICommandSender sender)
		{
			if (sender is Player pl)
				if (!string.IsNullOrEmpty(pl.SteamId))
					return true;
				else
					return false;
			else
				return false;
		}

		string ColoredRole(Player player)
		{
			switch ((Team)player.TeamRole.Team)
			{
				case Team.SCP:
					return "@#fg=red;" + player.TeamRole.Name + "</color>";
				case Team.MTF:
					return "@#fg=blue;" + player.TeamRole.Name + "</color>";
				case Team.CHI:
					return "@#fg=green;" + player.TeamRole.Name + "</color>";
				case Team.RSC:
					return "@#fg=silver;" + player.TeamRole.Name + "</color>";
				case Team.CDP:
					return "@#fg=orange;" + player.TeamRole.Name + "</color>";
				case Team.TUT:
					return "<color=lime>" + player.TeamRole.Name + "</color>";
				default:
					return player.TeamRole.Name;
			}
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (Server.GetPlayers().Count > 0)
			{
				Player myPlayer = (args.Length > 0) ? API.GetPlayerFromString.GetPlayer(args[0]) : null;
				if (myPlayer == null && sender is Player sendingPlayer)
					myPlayer = sendingPlayer;
				else if (myPlayer == null)
					if (args.Length > 0)
						return new string[] { "Could not find player: " + args[0] };
					else
						return new string[] { GetUsage() };

				//Handling player stats
				AdminToolbox.AddMissingPlayerVariables(new List<Player> { myPlayer });
				AdminToolbox.atfileManager.PlayerStatsFileManager(new List<Player> { myPlayer }, Managers.ATFileManager.PlayerFile.Write);
				API.PlayerSettings playerDict = (AdminToolbox.ATPlayerDict.ContainsKey(myPlayer.SteamId)) ? AdminToolbox.ATPlayerDict[myPlayer.SteamId] : new API.PlayerSettings(myPlayer.SteamId);
				
				//Inventory
				string playerInv = string.Empty;
				foreach(Smod2.API.Item i in myPlayer.GetInventory().Where(i => i.ItemType != ItemType.NULL))
					playerInv += i.ItemType + ", ";
				if (playerInv == string.Empty) playerInv = "Empty Inventory";

				//Calculating remaining jail time
				int remainingJailTime = ((int)playerDict.JailedToTime.Subtract(DateTime.Now).TotalSeconds >= 0) ? (int)playerDict.JailedToTime.Subtract(DateTime.Now).TotalSeconds : 0;

				string _playerRole = (IsPlayer(sender)) ? ColoredRole(myPlayer) : myPlayer.TeamRole.Role + "";

				//Building string
				string playerInfoString = Environment.NewLine + Environment.NewLine +
						"Player: (" + myPlayer.PlayerId + ") " + myPlayer.Name + Environment.NewLine +
					BuildTwoLiner(" - SteamID: " + myPlayer.SteamId,								" - IP: " + myPlayer.IpAddress.Replace("::ffff:", string.Empty)) + Environment.NewLine +
					BuildTwoLiner(" - Server Rank: " + "<color=" + myPlayer.GetUserGroup().Color + ">" + myPlayer.GetRankName() + "</color>") + Environment.NewLine +
					BuildTwoLiner(" - Role: " + _playerRole,								" - Health: " + myPlayer.GetHealth()) + Environment.NewLine +
					BuildTwoLiner(" - AdminToolbox Toggables: ") + Environment.NewLine +
					BuildTwoLiner("   - Godmode: " + (playerDict.godMode),							" - NoDmg: " + (playerDict.dmgOff)) + Environment.NewLine +
					BuildTwoLiner("   - OverwatchMode: " + (myPlayer.OverwatchMode),				" - KeepSettings: " + (playerDict.keepSettings)) + Environment.NewLine +
					BuildTwoLiner("   - BreakDoors: " + (playerDict.destroyDoor),					" - PlayerLockDown: " + (playerDict.lockDown)) + Environment.NewLine +
					BuildTwoLiner("   - InstantKill: " + (playerDict.instantKill),					" - GhostMode: " + myPlayer.GetGhostMode()) + Environment.NewLine +
					BuildTwoLiner("   - IsJailed: " + (playerDict.isJailed),						" - Released In: " + remainingJailTime) + Environment.NewLine +
					BuildTwoLiner(" - Stats:") + Environment.NewLine +
					BuildTwoLiner("   - Kills: " + playerDict.Kills,								" - TeamKills: " + playerDict.TeamKills) + Environment.NewLine +
					BuildTwoLiner("   - Deaths: " + playerDict.Deaths,								" - Times Banned: " + playerDict.banCount) + Environment.NewLine +
					BuildTwoLiner("   - Playtime: " + (int)playerDict.MinutesPlayed + " minutes",	" - Rounds Played: " + playerDict.RoundsPlayed) + Environment.NewLine +
					BuildTwoLiner(" - Position:") + Environment.NewLine +
					BuildTwoLiner("  - X:" + (int)myPlayer.GetPosition().x + " Y:" + (int)myPlayer.GetPosition().y + " Z:" + (int)myPlayer.GetPosition().z) + Environment.NewLine +
					BuildTwoLiner(" - Inventory: " + playerInv) + Environment.NewLine;
				if (IsPlayer(sender))
					return new string[] { playerInfoString.Replace("True ", "<color=green>" + "True" + " </color>").Replace("False", "<color=red>" + "False" + "</color>") };
				else
					return new string[] { playerInfoString };
			}
			else
				return new string[] { "Server is empty!" };
		}
	}
}
