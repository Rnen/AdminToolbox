using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class EmptyCommand : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "Empties the player's inventory";
		}

		public string GetUsage()
		{
			return "E / EMPTY [Player] (ItemType INT / Delete) (Delete)";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			Server server = PluginManager.Manager.Server;
			if (args.Length > 0)
			{
				Player myPlayer = API.GetPlayerFromString.GetPlayer(args[0]);
				if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
				if (myPlayer.TeamRole.Role == Smod2.API.Role.UNASSIGNED) return new string[] { "Player not properly initialized!" };
				AdminToolbox.AddMissingPlayerVariables(myPlayer);
				byte itemNumber = 0;
				string[] deleteAliases = { "delete", "del", "d" };
				if (args.Length > 1 && deleteAliases.Contains(args[1].ToLower()))
				{
					foreach (Smod2.API.Item item in myPlayer.GetInventory())
						if (item.ItemType != ItemType.NULL) { item.Remove(); itemNumber++; }
					myPlayer.SetAmmo(AmmoType.DROPPED_5, 0);
					myPlayer.SetAmmo(AmmoType.DROPPED_7, 0);
					myPlayer.SetAmmo(AmmoType.DROPPED_9, 0);
					return new string[] { "Deleted " + itemNumber + " items from player " + myPlayer.Name + "'s inventory" };
				}
				else if (args.Length > 1 && byte.TryParse(args[1], out byte itemInt))
					if (args.Length > 2 && deleteAliases.Contains(args[2].ToLower()))
					{
						foreach (Smod2.API.Item item in myPlayer.GetInventory())
							if ((byte)item.ItemType == itemInt) { item.Remove(); itemNumber++; }
						return new string[] { "Deleted all \"" + Enum.GetName(typeof(ItemType), itemInt) + "\" items from player " + myPlayer.Name + "'s inventory" };
					}
					else
					{
						foreach (Smod2.API.Item item in myPlayer.GetInventory())
							if ((byte)item.ItemType == itemInt) { item.Drop(); itemNumber++; }
						return new string[] { "Dropped all \"" + Enum.GetName(typeof(ItemType), itemInt) + "\" items from player " + myPlayer.Name + "'s inventory" };
					}
				else
				{
					foreach (Smod2.API.Item item in myPlayer.GetInventory())
						if (item.ItemType != ItemType.NULL) { item.Drop(); itemNumber++; }
					PluginManager.Manager.Server.Map.SpawnItem(ItemType.DROPPED_5, myPlayer.GetPosition(), myPlayer.GetRotation());
					PluginManager.Manager.Server.Map.SpawnItem(ItemType.DROPPED_7, myPlayer.GetPosition(), myPlayer.GetRotation());
					PluginManager.Manager.Server.Map.SpawnItem(ItemType.DROPPED_9, myPlayer.GetPosition(), myPlayer.GetRotation());
					myPlayer.SetAmmo(AmmoType.DROPPED_5, 0);
					myPlayer.SetAmmo(AmmoType.DROPPED_5, 0);
					myPlayer.SetAmmo(AmmoType.DROPPED_5, 0);
					return new string[] { "Dropped " + itemNumber + " items from player " + myPlayer.Name + "'s inventory" };
				}
			}
			return new string[] { GetUsage() };
		}
	}
}