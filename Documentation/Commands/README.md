### Note that all commands below requires you to have `enable_ra_server_commands:` set to true in your `config_gameplay.txt`, and your steamID64/SERVER-ROLE added to the whitelist for them to work. This is part of [Smod](https://github.com/Grover-c13/Smod2), not the plugin.
***
## Administration COMMANDS / Gameplay COMMANDS
Command | Value Type | Value Type | Description
--- | :---: | :---: | ---
P / PLAYER | Player |    | Lists all values for the specified Player
PLAYERS |   |    | Lists all players + IP's + SteamID's.
[ATBAN](Commands/Player/AtBan.md) | | | For banning offline user's IP/SteamID. See command's page for more info
ATGOD | Player | Boolean |  Turns on/off godmode for that player. Use without (bool) to toggle. Add `nodmg` after (bool) to turn on NoDmg as as well for this player
NODMG | Player | Boolean | Turns on/off damageOutput for that player. Use without (bool) to toggle.
ATHP / ATSETHP | Player | Integer  | Sets player HP to (Integer).
ATHEAL | Player | Integer  | Heals player for (Integer) amount. Use without (Integer) to set to Role default max HP.
TPX | Player1 | Player2  | Teleports Player1 to Player2. (Will not grab all players names that contain the string you entered like TP does, only the closest match)
TUT / TUTORIAL | Player  |  | Sets player role to TUTORIAL.
ROLE  | Player | ROLEID |  Sets player to ROLE without teleporting them to ROLE SPAWN or giving items.
KEEP / KEEPSETTINGS | Player| Boolean  | Enables keeping player settings on round restart.
RLOCK / ROUNDLOCK | Boolean |    | Turns on/off round ending
BD / BREAKDOORS | Player | Boolean |  Turns on/off breaking doors for that player.
PL / PLOCK / PLAYERLOCK / PLAYERLOCKDOWN | Player | Boolean |  Locks all doors for the specified player.
SPEC / SPECTATOR | Player | Boolean |  Player will never spawn naturally.
IK / INSTAKILL / INSTANTKILL | Player | Boolean |  Turns on/off instant kills for that player.
ATDISABLE | | |  **Disables the Admintoolbox plugin.** Server will need restart to enable it again
ATCOLOR | Boolean | |  `Enable/Disable` admintoolbox colors in server console (currently bugged)
JAIL | Player | seconds |  Jails the player for the specified (if not specified, defaults to 1 year) time. Re-use to unjail.
S / SERVERINFO |  | | Lists information on the server, such as the name, IP, port, player count, round number and duration, admintoolbox coloring, roundlock and jailed players
KILL / SLAY | Player | | Kills target player. Using `*` will exclude the player using the command
ATSPEAK / ATINTERCOM / AT-SPEAK | Player | | Sets player as Intercom Speaker. Use without Player to set yourself. If used while broadcast, the current broadcast will cancel.
ATHELP | | | Opens this GitHub page for the user
AT | HELP / INFO / DOWNLOAD |  | `INFO` compares the server's AT version with the latest GitHub one. `DOWNLOAD` opens a new browser page for the newest download
SERVERSTATS / ROUNDSTATS | |  | Displays each type of victory since last server (re)boot
[GHOSTMODE](Commands/Player/GhostMode.md) | | | For becoming invisible

>Any `Player` variable can be switched out with `*` to target all players. Not specifying a bool toggles it.
>Using `(command) list` will list all players with the currently enabled status. (Like godmode for example)

>Find a complete list of Role ID's & Item ID's [HERE](Resources.md)
***
### Advanced Commands (Theese are harder to use and/or requires more Values)
Command | Value Type | Value Type |  Value Type | Description
--- | :---: | :---: | :---: | ---
WARP | ADD | Player | WarpName | Adds a warp point where `Player` is
WARP | REMOVE | WarpName |   | Removes `WarpName` from WarpPoints
WARP | LIST |   |   |  Lists current warp points
WARP | Player | WarpName |   |  Teleports `Player` to `WarpName`
POS | Player | ADD  |  x=5 y=10 | Teleports player 5 on X axis, 10 on Y axis (up).  (No need to use every coordinate (X Y Z))
POS | Player | SET  |  x=50 y=0 z=-50  | Sets player position to X:50 Y:0 Z:-50
POS | Player | GET | |  Gets XYZ position of `Player`
EMPTY | Player | ItemTypeNumber / (D, DEL,DELETE) | (D, DEL,DELETE) | Drops all items (all unless specified) from the player's inv. If used with (D, DEL or DELETE), it deletes the items instead of dropping. (The second del part is for if you use an item number)


> Player input doesn't need the full name, it will grab the closest matching name containing your entry
***
## ^These commands work in both server console and text-based remote admin!^