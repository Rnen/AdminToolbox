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
		private static Map Map => PluginManager.Manager.Server.Map;

		public string GetCommandDescription() => "Empties the player's inventory";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [Player] (ItemType Number / Delete) <Delete>";

		public static readonly string[] CommandAliases = new string[] { "EMPTY", "E" };

		private readonly string[] deleteAliases = { "DELETE", "DEL", "D" };

#warning EmptyCommand needs proper testing
		public string[] OnCall(ICommandSender sender, string[] args) //=> new string[] { GetUsage() };
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				ItemType type = ItemType.NULL;
				Player player = null;
				bool delete = false;

				foreach(string arg in args)
				{
					if (player == null && GetPlayerFromString.GetPlayer(arg) is Player p && p != null)
					{
						player = p;
						continue;
					}
					if (int.TryParse(arg, out int itemNumber))
					{
						Utility.TryParseItem(itemNumber, out type);
						continue;
					}
					if (deleteAliases.Contains(arg.ToUpper()))
					{
						delete = true;
						continue;
					}
				}
				return DropItems(player ?? sender as Player, type, delete);
			}
			else
				return deniedReply;
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
			return new string[] { delete ? "Deleted " : "Dropped (" + itemCount + ") items from player " + player.Name + "'s inventory" };
		}
	}
}
