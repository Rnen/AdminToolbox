using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Linq;
using System.Collections.Generic;
using SMItem = Smod2.API.Item;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;

	public class EmptyCommand : ICommandHandler
	{
		private static Server Server => PluginManager.Manager.Server;
		private static Map Map => Server.Map;

		public string GetCommandDescription() => "Empties the player's inventory";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [Player] (ItemType Number / Delete) <Delete>";

		public static readonly string[] CommandAliases = new string[] { "EMPTY", "E" };

		private readonly string[] deleteAliases = { "DELETE", "DEL", "D" };
		private readonly int[] enumValues = Enum.GetValues(typeof(ItemType)).Cast<int>().ToArray();

#error Fix EmptyCommand
		public string[] OnCall(ICommandSender sender, string[] args) //=> new string[] { GetUsage() };
		{
			
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				int itemNumber;
				ItemType type = ItemType.NULL;

				switch (args.Length)
				{
					case 0 when !(sender is Player):
						return new string[] { GetUsage() };
					case 0 when sender is Player p:
						return DropItems(p);
					case 1 when deleteAliases.Contains(args[0].ToUpper()):
							return DropItems(sender as Player, delete: true);
					case 1 when int.TryParse(args[0], out itemNumber) && Utility.TryParseItem(itemNumber, out type) && sender is Player:
							return DropItems(sender as Player, type);
					case 1 when GetPlayerFromString.GetPlayer(args[0]) is Player p2 && p2 != null:
						return DropItems(p2);
					case 2 when GetPlayerFromString.GetPlayer(args[0]) is Player p3 && p3 != null:
						if (int.TryParse(args[1], out itemNumber) && Utility.TryParseItem(itemNumber, out type))
							return DropItems(p3, type);
						else if ()
					default:
						if (int.TryParse(args[0], out itemNumber) && Utility.TryParseItem(itemNumber, out type))
							return DropItems(GetPlayerFromString.GetPlayer(args[0]), type, deleteAliases.Contains(args[2].ToUpper()));
						else
							return new string[] { GetUsage() };
				}
			}
			else
				return deniedReply;

			return new string[] { GetUsage() };
		}


		private string[] DropItems(Player player, ItemType itemFilter = ItemType.NULL, bool delete = false)
		{
			ItemType ammoFlag = ItemType.DROPPED_5 | ItemType.DROPPED_7 | ItemType.DROPPED_9;

			if (player == null)
				return new string[] { "Player not spesified!" };
			if (player.TeamRole.Role == Role.UNASSIGNED)
				return new string[] { "Player not properly initialized!" };
			if (player.TeamRole.Role == Role.SPECTATOR)
				return new string[] { "This can not be used on spectators!" };

			byte itemCount = 0;
			Vector pos = player.GetPosition(), rot = player.GetRotation();

			if (itemFilter == ItemType.NULL || !ammoFlag.HasFlag(itemFilter))
			{
				foreach (SMItem playerItem in player.GetInventory())
				{
					if (playerItem.ItemType != ItemType.NULL)
						if (itemFilter == ItemType.NULL || playerItem.ItemType == itemFilter)
						{
							if (delete)
								playerItem.Remove();
							else
								playerItem.Drop();
							itemCount++;
						}
				}


			}

			if (itemFilter == ItemType.NULL || ammoFlag.HasFlag(itemFilter))
			{
				foreach (AmmoType ammo in Enum.GetValues(typeof(AmmoType)))
				{
					ItemType ammoItem = ammo == AmmoType.DROPPED_5 ? ItemType.DROPPED_5 : ammo == AmmoType.DROPPED_7 ? ItemType.DROPPED_7 : ItemType.DROPPED_9;

					if (itemFilter == ItemType.NULL || ammoItem == itemFilter)
					{
						player.SetAmmo(ammo, 0);
						if (!delete)
							Map.SpawnItem(ammoItem, pos, rot);
					}
				}
			}
			return new string[] { delete ? "Deleted " : "Dropped" + itemCount + " items from player " + player.Name + "'s inventory" };
		}
	}
}
