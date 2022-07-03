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