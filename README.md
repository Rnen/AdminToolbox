# AdminToolbox
This is a plugin for SCL:Secret Lab servers. 
It offers multiple admin tools like 
* Friendly fire logs in server console
* Godmode, ability to turn off damage for players
* Heal & SetHP
* Warp Points
* Teleport to cordinates
* Prevent the round from ending
* Per-player stats
* Breaking doors interacted with
* Player Joined messages in server log

and more commands, and other features that do not exist in the base game (yet).
There is also more to come!

If you want to follow the development, take a look at the [AdminToolbox Trello Board](https://trello.com/b/NnNBzSX2/scpsl-admintoolbox-plugin)

## Plugin Installation:
To install:
1. Grab newest version of AdminToolbox: [Latest Release](https://github.com/Rnen/AdminToolbox/releases/latest)
2. Navigate to your SCP Secret Lab folder.
3. Drag AdminToolbox.dll into the sm_plugins folder

## ServerMod - (This is required for any plugin)
ServerMod is a server side plugin system with a bunch of additional configuration options, bug fixes, security patches and some optimisations built in.
 * SMOD can be found here: [Smod Github](https://github.com/Grover-c13/Smod2)
 * SMOD Discord: https://discord.gg/8nvmMTr

### Note that all commands below requires you to have `enable_ra_server_commands:` set to true in your `config_gameplay.txt`, and your steamID64/SERVER-ROLE added to the whitelist for them to work. This is part of [Smod](https://github.com/Grover-c13/Smod2), not the plugin.
## Administration COMMANDS / Gameplay COMMANDS
Command | Value Type | Value Type | Value Type | Description
--- | :---: | :---: | ---
P / PLAYER | Player |    | Lists all values for the specified Player
PLAYERS |   |    | Lists all players + IP's + SteamID's.
GOD | Player | Boolean |  Turns on/off godmode for that player. Use without (bool) to toggle. Add `nodmg` after (bool) to turn on NoDmg as as well for this player
NODMG | Player | Boolean | Turns on/off damageOutput for that player. Use without (bool) to toggle.
HP / SETHP | Player | Integer  | Sets player HP to (Integer).
HEAL | Player | Integer  | Heals player for (Integer) amount. Use without (Integer) to set to Role default max HP.
TPX | Player1 | Player2  | Teleports Player1 to Player2. (Will not grab all players names that contain the string you entered like TP does, only the closest match)
TUT / TUTORIAL | Player  |  | Sets player role to TUTORIAL.
ROLE  | Player | ROLEID |  Sets player to ROLE without teleporting them to ROLE SPAWN or giving items.
KEEP / KEEPSETTINGS | Player| Boolean  | Enables keeping player settings on round restart.
RLOCK / ROUNDLOCK | Boolean |    | Turns on/off round ending
BD / BREAKDOORS | Player | Boolean |  Turns on/off breaking doors for that player.
PL / PLOCK / PLAYERLOCK / PLAYERLOCKDOWN | Player | Boolean |  Locks all doors for the spesified player.
SPEC / SPECTATOR | Player | Boolean |  Player will never spawn naturally.
IK / INSTAKILL / INSTANTKILL | Player | Boolean |  Turns on/off instant kills for that player.
ATDISABLE | | |  **Disables the Admintoolbox plugin.** Server will need restart to enable it again
ATCOLOR | Boolean | |  `Enable/Disable` admintoolbox colors in server console (currently bugged)
JAIL | Player | seconds |  Jails the player for the specified (if not specified, defaults to 1 year) time. Re-use to unjail.
S / SERVERINFO |  Lists information on the server, such as the name, IP, port, player count, round number and duration, admintoolbox coloring, roundlock and jailed players

>Any `Player` variable can be switched out with `*` to target all players. Not specifying a bool toggles it.
>Using `(command) list` will list all players with the currently enabled status. (Like godmode for example)

>Find a complete list of Role ID's & Item ID's [HERE](https://github.com/Rnen/AdminToolbox/blob/master/.github/RESOURCES.md)

### Advanced Commands (Theese are harder to use and/or requires more Values)
Command | Value Type | Value Type |  Value_Type | Description
--- | :---: | :---: | :---: | ---
WARP | ADD | Player | WarpName | Adds a warp point where `Player` is
WARP | REMOVE | WarpName |   | Removes `WarpName` from WarpPoints
WARP | LIST |   |   |  Lists current warp points
WARP | Player | WarpName |   |  Teleports `Player` to `WarpName`
POS | Player | ADD  |  x=5 y=10 | Teleports player 5 on X axis, 10 on Y axis (up).  (No need to use every cordinate (X Y Z))
POS | Player | SET  |  x=50 y=0 z=-50  | Sets player position to X:50 Y:0 Z:-50
POS | Player | GET | |  Gets XYZ position of `Player`
EMPTY | Player | ItemTypeNumber / (D, DEL,DELETE) | (D, DEL,DELETE) | Drops all items (all unless specified) from the player's inv. If used with (D, DEL or DELETE), it deletes the items instead of dropping.

> Player input doesn't need the full name, it will grab the closest matching name containing your entry
## ^These commands work in both server console and text-based remote admin!^

## Config Additions
Type Info:
- Boolean: True or False value
- Integer: Any whole number
- Float: A number with decimals (Formatting like "1.5")
- List: A list of items separated by ",", for example: `list: 1,2,3,4,5`
- Dictionary: A dictionary of items separated by ":", and each entry separated by ",", for example: `dictionary: 1:2,2:3,3:4`
- Seconds: Time in seconds, usually a value of -1 disables the feature
- Minutes: Time in minutes, usually a value of -1 disables the feature
- R: If the config option has an R before it, it means that you can use a random value in it. A random value is defined by having "{}", items listed like "weight%value" where if you don't put a weight it defaults to a weight of 1, separated by "|", for example: `rlist: {1%1|2%7|6},3,6,{15%3|2|45%2}`
- STEAMID64: [Find yours here!](https://steamid.xyz/)

>Crossed out config options are removed, unless otherwise specified in the description

## If you do not intend to change the default values, there's no need to include any of theese in your config
### General Settings
Config Option | Value Type | Default Value | Description
--- | :---: | :---: | ---
admintoolbox_enable | Boolean | True | `Enable / Disable` AdminToolbox from loading on server start
admintoolbox_colors | Boolean | False | `Enable/Disable` admintoolbox colors in server console (currently bugged)
admintoolbox_tutorial_dmg_allowed | List | -1 | What damage types the TUTORIAL role is allowed to take. -1 means no damagetypes allowed
admintoolbox_Round_damageMultiplier | Float | 1 | Multiplies all damage by this number
+admintoolbox_endedRound_damageMultiplier | Float | 1 | Multiplies all damage by this number after round ends. For maximum chaos enter high number (10 or something) To turn off dmg on round end, enter `0`.
admintoolbox_decontamination_damagemultiplier | Float | 1 | Multiplies LCZ decontaimnent damage with the specified number
admintoolbox_debug_player_player_joinANDleave | Boolean | False | Writes Playername in server console when player joins (Currently only works for players joining, not leaving)
~~admintoolbox_intercom_whitelist~~ | ~~(serverRole:SpeakTime-CooldownTime)~~ | | ~~Whitelist of server roles (roles from `config_remoteadmin.txt` with specific time settings~~ **Temporarily removed**
admintoolbox_intercom_steamid_blacklist | SteamID64 |  | List of people who cannot use the intercom
admintoolbox_intercomlock | Boolean | False | If true locks the intercom for non-whitelisted players
admintoolbox_block_role_damage | Dictionary | 2:2 | What roles cannot damage other roles. See example under
admintoolbox_round_info | Boolean | True | Displays round-count and dudation on start/end of round

### Logfile settings (file will be created in the `AppData\Roaming\SCP Secret Laboratory\ATServerLogs` folder
Config Option | Value Type | Default Value | Description
--- | :---: | :---: | ---
admintoolbox_log_teamkills | Boolean | False | Writes teamkills to the AT logfile
admintoolbox_log_kills | Boolean | False | Writes non-team kills to the AT logfile
admintoolbox_log_commands | Boolean | False | Writes command usage to the AT logfile

### Debug Settings (If you do not intend to change the default values, theres no need to include any of theese in your config)
Config Option | Value Type | Default Value | Description
--- | :---: | :---: | ---
admintoolbox_debug_damagetypes | List | All human player damage ID's | What damage types to detect. 
admintoolbox_debug_player_damage | Boolean | False | Displays all non-friendly kills in server console.
admintoolbox_debug_friendly_damage | Boolean | False | Displays team damage in server console.
admintoolbox_debug_player_kill | Boolean | False | Displays all non-friendly kills in server console.
admintoolbox_debug_scp_and_self_killed  | Boolean | False | Displays suicides, granade kills and SCP kills in server console.
admintoolbox_debug_friendly_kill | Boolean | True | Displays teamkills in server console.
### *Note that all configs should go in your server config file, not config_remoteadmin.txt
Examples:
```yaml
#The example under will make scientists and Chaos unable to damage Dboys. The second example is how you add several roleID's for one role. (The example would make scientists (6) not able to damage any class, dboys (1) not able to attack other dboys (1))
admintoolbox_block_role_damage: 6:0-1-2-3-4-5-6-7-8-9-10-11-12-13-14-15-16-17,1:1

```
>Find a complete list of DamageID's, RoleID's and more [HERE](https://github.com/Rnen/AdminToolbox/blob/master/.github/RESOURCES.md)
#

### Place any suggestions/problems in [issues](https://github.com/Rnen/AdminToolbox/issues)!

# Thanks & Enjoy.
