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
			return "(P / PLAYER) [PLAYERNAME]";
		}
		private static string StringToMax(string text, int max = 29)
		{
			while (text.Length < max)
				text += ' ';
			return text;
		}
		string BuildTwoLiner(string str1, string str2 = "")
		{
			return StringToMax(str1) + "|" + StringToMax(str2.PadLeft(LeftPadding));
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
			string ColoredBools(bool input)
			{
				if (isPlayer(sender) && input)
					return "<color=green>" + input + " </color>";
				else if (isPlayer(sender) && !input)
					return "<color=red>" + input + "</color>";
				else if (input)
					return input.ToString() + " ";
				else
					return input.ToString();
			}

			if (server.GetPlayers().Count > 0)
			{
				Player myPlayer = (args.Length > 0) ? GetPlayerFromString.GetPlayer(args[0]) : null;
				if (args.Length == 0)
					if (sender is Player sendingPlayer)
						myPlayer = sendingPlayer;
				if (myPlayer == null)
					return new string[] { "Couldn't get player: " + args[0] };

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
					BuildTwoLiner("   - Godmode: " + ColoredBools(playerDict.godMode),				"  - NoDmg: " + ColoredBools(playerDict.dmgOff)) + Environment.NewLine +
					BuildTwoLiner("   - SpectatorOnly: " + ColoredBools(playerDict.spectatorOnly),	"  - KeepSettings: " + ColoredBools(playerDict.keepSettings)) + Environment.NewLine +
					BuildTwoLiner("   - BreakDoors: " + ColoredBools(playerDict.destroyDoor),		"  - PlayerLockDown: " + ColoredBools(playerDict.lockDown)) + Environment.NewLine +
					BuildTwoLiner("   - InstantKill: " + ColoredBools(playerDict.instantKill)) + Environment.NewLine +
					BuildTwoLiner("   - IsJailed: " + ColoredBools(playerDict.isJailed),			" - Released In: " + remainingJailTime) + Environment.NewLine +
					BuildTwoLiner(" - Stats:") + Environment.NewLine +
					BuildTwoLiner("     - Kills: " + playerDict.Kills,								"- TeamKills: " + playerDict.TeamKills) + Environment.NewLine +
					BuildTwoLiner("     - Deaths: " + playerDict.Deaths,							"- Times Banned: " + playerDict.banCount) + Environment.NewLine +
					BuildTwoLiner("     - Playtime: " + playerDict.minutesPlayed + " minutes",		"- Rounds Played: " + playerDict.RoundsPlayed) + Environment.NewLine +
					BuildTwoLiner(" - Position:") + Environment.NewLine +
					BuildTwoLiner(" - X:" + (int)myPlayer.GetPosition().x + " Y:" + (int)myPlayer.GetPosition().y + " Z:" + (int)myPlayer.GetPosition().z) + Environment.NewLine +
					BuildTwoLiner(" - Inventory: " + playerInv) + Environment.NewLine;

				return new string[] { playerInfoString };
			}
			else
				return new string[] { "Server is empty!" };
		}
	}
}
