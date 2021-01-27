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
				Smod2.API.ItemType type = Smod2.API.ItemType.NULL;
				Player player = null;
				bool delete = false;

				foreach (string arg in args)
				{
					if (player == null && GetPlayerFromString.TryGetPlayer(arg, out Player p))
					{
						player = p;
						continue;
					}
					if (type == Smod2.API.ItemType.NULL && int.TryParse(arg, out int itemNumber))
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


		private string[] DropItems(Player player, Smod2.API.ItemType itemFilter = Smod2.API.ItemType.NULL, bool delete = false)
		{
			Smod2.API.ItemType ammoFlag = Smod2.API.ItemType.DROPPED_5 | Smod2.API.ItemType.DROPPED_7 | Smod2.API.ItemType.DROPPED_9;

			if (player == null)
				return new string[] { "Player not spesified!" };
			if (player.TeamRole.Role == Smod2.API.RoleType.UNASSIGNED)
				return new string[] { "Player not properly initialized!" };
			if (player.TeamRole.Role == Smod2.API.RoleType.SPECTATOR)
				return new string[] { "This can not be used on spectators!" };

			byte itemCount = 0;
			Vector pos = player.GetPosition(), rot = player.GetRotation();

			if (itemFilter == Smod2.API.ItemType.NULL || !ammoFlag.HasFlag(itemFilter))
			{
				foreach (SMItem playerItem in player.GetInventory())
				{
					if (playerItem.ItemType != Smod2.API.ItemType.NULL)
						if (itemFilter == Smod2.API.ItemType.NULL || playerItem.ItemType == itemFilter)
						{
							if (delete)
								playerItem.Remove();
							else
								playerItem.Drop();
							itemCount++;
						}
				}


			}

			if (itemFilter == Smod2.API.ItemType.NULL || ammoFlag.HasFlag(itemFilter))
			{
				foreach (AmmoType ammo in Enum.GetValues(typeof(AmmoType)))
				{
					Smod2.API.ItemType ammoItem = ammo == Smod2.API.AmmoType.DROPPED_5 ? Smod2.API.ItemType.DROPPED_5 : ammo == Smod2.API.AmmoType.DROPPED_7 ? Smod2.API.ItemType.DROPPED_7 : Smod2.API.ItemType.DROPPED_9;

					if (itemFilter == Smod2.API.ItemType.NULL || ammoItem == itemFilter)
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
