using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Linq;
using System.Collections.Generic;
using AdminToolbox.API;

namespace AdminToolbox.Command
{
	class EmptyCommand : ICommandHandler
	{
		private static Map Map => PluginManager.Manager.Server.Map;

		public string GetCommandDescription() => "Empties the player's inventory";
		public string GetUsage() => "E / EMPTY [Player] (ItemType Number / Delete) (Delete)";
		

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			Server server = PluginManager.Manager.Server;
			int max = Enum.GetValues(typeof(ItemType)).Cast<int>().Max(), 
				min = Enum.GetValues(typeof(ItemType)).Cast<int>().Min() + 1;
			if (args.Length > 0)
			{
				Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
				if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
				if (myPlayer.TeamRole.Role == Role.UNASSIGNED) return new string[] { "Player not properly initialized!" };
				if (myPlayer.TeamRole.Role == Role.SPECTATOR) return new string[] { "This can not be used on spectators!" };
				AdminToolbox.AddMissingPlayerVariables(myPlayer);

				byte itemNumber = 0;
				Vector pos = myPlayer.GetPosition(), rot = myPlayer.GetRotation();
				string[] deleteAliases = { "delete", "del", "d" };
				if (args.Length > 1 && deleteAliases.Contains(args[1].ToLower()))
				{
					foreach (Smod2.API.Item item in myPlayer.GetInventory().Where(i => i.ItemType != ItemType.NULL))
						 { item.Remove(); itemNumber++; }
					foreach (int a in Enum.GetValues(typeof(AmmoType)))
						myPlayer.SetAmmo((AmmoType)a, 0);
					return new string[] { "Deleted " + itemNumber + " items from player " + myPlayer.Name + "'s inventory" };
				}
				else if (args.Length > 1 && byte.TryParse(args[1], out byte itemInt) && itemInt <= max && itemInt >= min)
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

					foreach (Smod2.API.Item item in myPlayer.GetInventory().Where(i => i.ItemType != ItemType.NULL))
						 { item.Drop(); itemNumber++; }
					Map.SpawnItem(ItemType.DROPPED_5, pos, rot);
					Map.SpawnItem(ItemType.DROPPED_7, pos, rot);
					Map.SpawnItem(ItemType.DROPPED_9, pos, rot);
					foreach (int a in Enum.GetValues(typeof(AmmoType)))
						myPlayer.SetAmmo((AmmoType)a, 0);
					return new string[] { "Dropped " + itemNumber + " items from player " + myPlayer.Name + "'s inventory" };
				}
			}
			else if(sender is Player p && p != null)
			{
				int itemNumber = 0;
				Vector pos = p.GetPosition(), rot = p.GetRotation();
				foreach (Smod2.API.Item item in p.GetInventory().Where(i => i.ItemType != ItemType.NULL))
				{ item.Drop(); itemNumber++; }
				Map.SpawnItem(ItemType.DROPPED_5, pos, rot);
				Map.SpawnItem(ItemType.DROPPED_7, pos, rot);
				Map.SpawnItem(ItemType.DROPPED_9, pos, rot);
				foreach (int a in Enum.GetValues(typeof(AmmoType)))
					p.SetAmmo((AmmoType)a, 0);
				return new string[] { "Dropped " + itemNumber + " items from player " + p.Name + "'s inventory" };
			}
			else
				return new string[] { GetUsage() };
		}
	}
}