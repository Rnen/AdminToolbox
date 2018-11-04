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
		int LeftPadding = 3;
		Server server = PluginManager.Manager.Server;

		public string GetCommandDescription()
		{
			return "Gets toolbox info about spesific player";
		}

		public string GetUsage()
		{
			return "(P / PLAYER) [PLAYERNAME/ID/STEAMID]";
		}
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

		bool isPlayer(ICommandSender sender)
		{
			if (sender is Player pl)
				if (!string.IsNullOrEmpty(pl.SteamId))
					return true;
				else
					return false;
			else
				return false;
		}


		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (server.GetPlayers().Count > 0)
			{
				Player myPlayer = (args.Length > 0) ? GetPlayerFromString.GetPlayer(args[0]) : null;
				if (myPlayer == null && sender is Player sendingPlayer)
					myPlayer = sendingPlayer;
				else if (myPlayer == null)
					if (args.Length > 0)
						return new string[] { "Couldn't get player: " + args[0] };
					else
						return new string[] { GetUsage() };

				//Handling player stats
				AdminToolbox.AddMissingPlayerVariables(new List<Player> { myPlayer });
				AdminToolbox.AdminToolboxLogger.PlayerStatsFileManager(new List<Player> { myPlayer }, LogHandlers.PlayerFile.Write);
				AdminToolbox.AdminToolboxPlayerSettings playerDict = (AdminToolbox.playerdict.ContainsKey(myPlayer.SteamId)) ? AdminToolbox.playerdict[myPlayer.SteamId] : new AdminToolbox.AdminToolboxPlayerSettings();
				
				//Inventory
				string playerInv = string.Empty;
				myPlayer.GetInventory().ForEach(i => { if (i.ItemType != ItemType.NULL) playerInv += i.ItemType + ", "; });
				if (playerInv == string.Empty) playerInv = "Empty Inventory";

				//Calculating remaining jail time
				int remainingJailTime = ((int)playerDict.JailedToTime.Subtract(DateTime.Now).TotalSeconds >= 0) ? (int)playerDict.JailedToTime.Subtract(DateTime.Now).TotalSeconds : 0;

				//Building string
				string playerInfoString = Environment.NewLine + Environment.NewLine +
						"Player: (" + myPlayer.PlayerId + ") " + myPlayer.Name + Environment.NewLine +
					BuildTwoLiner(" - SteamID: " + myPlayer.SteamId,								" - IP: " + myPlayer.IpAddress) + Environment.NewLine +
					BuildTwoLiner(" - Server Rank: " + "<color=" + myPlayer.GetUserGroup().Color + ">" + myPlayer.GetRankName() + "</color>") + Environment.NewLine +
					BuildTwoLiner(" - Role: " + myPlayer.TeamRole.Role,								" - Health: " + myPlayer.GetHealth()) + Environment.NewLine +
					BuildTwoLiner(" - AdminToolbox Toggables: ") + Environment.NewLine +
					BuildTwoLiner("   - Godmode: " + (playerDict.godMode),							" - NoDmg: " + (playerDict.dmgOff)) + Environment.NewLine +
					BuildTwoLiner("   - OverwatchMode: " + (myPlayer.OverwatchMode),				" - KeepSettings: " + (playerDict.keepSettings)) + Environment.NewLine +
					BuildTwoLiner("   - BreakDoors: " + (playerDict.destroyDoor),					" - PlayerLockDown: " + (playerDict.lockDown)) + Environment.NewLine +
					BuildTwoLiner("   - InstantKill: " + (playerDict.instantKill),					" - GhostMode: " + myPlayer.GetGhostMode()) + Environment.NewLine +
					BuildTwoLiner("   - IsJailed: " + (playerDict.isJailed),						" - Released In: " + remainingJailTime) + Environment.NewLine +
					BuildTwoLiner(" - Stats:") + Environment.NewLine +
					BuildTwoLiner("   - Kills: " + playerDict.Kills,								" - TeamKills: " + playerDict.TeamKills) + Environment.NewLine +
					BuildTwoLiner("   - Deaths: " + playerDict.Deaths,								" - Times Banned: " + playerDict.banCount) + Environment.NewLine +
					BuildTwoLiner("   - Playtime: " + (int)playerDict.minutesPlayed + " minutes",	" - Rounds Played: " + playerDict.RoundsPlayed) + Environment.NewLine +
					BuildTwoLiner(" - Position:") + Environment.NewLine +
					BuildTwoLiner("  - X:" + (int)myPlayer.GetPosition().x + " Y:" + (int)myPlayer.GetPosition().y + " Z:" + (int)myPlayer.GetPosition().z) + Environment.NewLine +
					BuildTwoLiner(" - Inventory: " + playerInv) + Environment.NewLine;
				if (isPlayer(sender))
					return new string[] { playerInfoString.Replace("True", "<color=green>" + "True " + " </color>").Replace("False", "<color=red>" + "False" + "</color>") };
				else
					return new string[] { playerInfoString };
			}
			else
				return new string[] { "Server is empty!" };
		}
	}
}
