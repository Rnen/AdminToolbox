# If you do not intend to change the default values, there's no need to include any of theese in your config

>Crossed out config options are temporarily removed, unless otherwise specified in the description

***

## Variable Types

- **Boolean:** True or False value
- **Integer:** Any whole number (non-decimal)
- **Float:** A number with decimals (Formatting like "1.5")
- **List:** A list of items separated by a `,` (comma). For example: `list: 1,2,3,4,5`
- **Dictionary:** A dictionary of items separated by ":", and each entry separated by ",", for example: `dictionary: 1:2,2:3,3:4`
- **Seconds:** Time in seconds, usually a value of -1 disables the feature
- **Minutes:** Time in minutes, usually a value of -1 disables the feature
- **R:** If the config option has an R before it, it means that you can use a random value in it. A random value is defined by having "{}", items listed like "weight%value" where if you don't put a weight it defaults to a weight of 1, separated by "|", for example: `rlist: {1%1|2%7|6},3,6,{15%3|2|45%2}`
- **STEAMID64:** Steam Profile number identifier [Find yours here!](https://steamid.xyz/)

***

### General Settings

Config Option | Value Type | Default Value | Description
--- | :---: | :---: | ---
admintoolbox_enable | Boolean | True | `Enable / Disable` AdminToolbox from loading on server start
admintoolbox_colors | Boolean | False | `Enable/Disable` admintoolbox colors in server console (currently bugged)
atb_disable_networking | Boolean | False | `True` disables all GitHub version checking.
admintoolbox_tracking | Boolean | True | Puts an invisible `AT:VersionNumber` in the server name
admintoolbox_round_info | Boolean | True | Displays round-count and duration on start/end of round
admintoolbox_player_join_info_extended | Boolean | True | Displays extended info about joining player
admintoolbox_player_join_info | Boolean | True | Displays joining player's name upon joining. (This does nothing if `extended` version is true)
***

### Damage Settings

Damage Config Option | Value Type | Default Value | Description
--- | :--- | :--- | ---:
admintoolbox_tutorial_dmg_allowed | List | -1 | What damage types the TUTORIAL role is allowed to take. -1 means no damagetypes allowed, `-2` means all. (Alternatively to -2 you can write `all` or `*`)
admintoolbox_round_damageMultiplier | Float | 1 | Multiplies all damage by this number
admintoolbox_endedRound_damageMultiplier | Float | 1 | Multiplies all damage by this number after round ends. For maximum chaos enter high number (10 or something) To turn off dmg on round end, enter `0`.
admintoolbox_decontamination_damagemultiplier | Float | 1 | Multiplies LCZ decontaimnent damage with the specified number
admintoolbox_block_role_damage | roleX:role1 |  |  Blocks damage between specified roles.
***

```yaml
#Example: Chaos cannot damage Dboys, scientists cannot harm any MTF. (But opposite is fine for both)
admintoolbox_block_role_damage: 8:1,6:12:13:15
```

***

### Info Settings

Config Option | Value Type | Default Value | Description
--- | :---: | :---: | ---
admintoolbox_info_player_join | Boolean | True | Writes Playername in server console when player joins (Currently only works for players joining, not leaving)
***

### Intercom Settings

Intercom Config Option | Value Type | Default Value | Description
--- | :---: | :---: | ---
admintoolbox_intercom_whitelist | ServerRole:SpeakTime:CooldownTime | | Whitelist of server roles (roles from `config_remoteadmin.txt` with specific time settings
admintoolbox_intercom_steamid_blacklist | SteamID64 |  | List of people who cannot use the intercom
admintoolbox_intercomlock | Boolean | False | If true locks the intercom for non-whitelisted players
admintoolbox_intercomtransmit_text | String | | What text the screen displays when transmitting (empty results in default game behaviour)
admintoolbox_intercomready_text | String | |  What text the screen displays when the intercom is ready to be used
admintoolbox_intercomrestart_text | String  | | What text the screen displays when its restarting (counting down)

Name variables for intercom_transmit config:
`$player` `$playerid` `$playerrole` `$playerteam` `$playerhp` (alternative `$playerhealth`) `$playerrank`
***

### Logfile settings

> File(s) will be created at the path specified in `admintoolbox_folder_path` config (SL Appdata Folder by default)

Config Option | Value Type | Default Value | Description
--- | :---: | :---: | ---
admintoolbox_log_teamkills | Boolean | False | Writes teamkills to the AT logfile
admintoolbox_log_kills | Boolean | False | Writes non-team kills to the AT logfile
admintoolbox_log_commands | Boolean | False | Writes command usage to the AT logfile
admintoolbox_log_damage | Boolean | False | Writes all player-damage to the AT logfile
admintoolbox_folder_path | String (Path) | %Appdata%\Roaming\SCP Secret Laboratory\ | Where the Admintoolbox folder will be located
admintoolbox_stats_unified | Boolean | True | If true uses one folder for all servers, false creates a folder per server
admintoolbox_logremover_hours_old | Int | 0 | Automaticly removes AT logs older than specified hours of age. 0 means disabled

***

### Debug Settings

Config Option | Value Type | Default Value | Description
--- | :---: | :---: | ---
admintoolbox_debug_damagetypes | List | All human player damage ID's | What damage types to detect. 
admintoolbox_debug_player_damage | Boolean | False | Displays all non-friendly kills in server console.
admintoolbox_debug_friendly_damage | Boolean | False | Displays team damage in server console.
admintoolbox_debug_player_kill | Boolean | False | Displays all non-friendly kills in server console.
admintoolbox_debug_scp_and_self_killed  | Boolean | False | Displays suicides, granade kills and SCP kills in server console.
admintoolbox_debug_friendly_kill | Boolean | True | Displays teamkills in server console.
***

### *Note that all configs should go in your server config file, not config_remoteadmin.txt

### Examples

```yaml
#The example would make scientists (6) not able to damage any class, dboys (1) not able to attack other dboys (1))
admintoolbox_block_role_damage: 6:0-1-2-3-4-5-6-7-8-9-10-11-12-13-14-15-16-17,1:1


admintoolbox_intercom_whitelist: owner:120:10, moderator:90:20
```

>Find a complete list of DamageID's, RoleID's and more in the [RESOURCES](Resources.md) page!

***

### Place any suggestions/problems in [issues](https://github.com/Rnen/AdminToolbox/issues)!

# Thanks & Enjoy.