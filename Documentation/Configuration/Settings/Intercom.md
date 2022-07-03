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