# ATBAN

Bans IP/STEAMID for X minutes. (For banning offline users)

## Command usage

* ATBAN ([string][csharp]: PlayerName/Label) ([string][csharp]: IP or SteamID) <[integer][csharp]: Minutes>

Parameter | Default State | Description
--: | :--: | :--
PlayerName / Label | | What shows where Player's name usually is in the ban file. Can be anything
IP / SteamID | | Either a steamID or a IP-adress
Minutes | 43800 | Defaults to 4 weeks when not specified otherwise

### Command Aliases

List of Aliases for the word `ATBAN`

* OBAN
* OFFLINEBAN

## Examples

Command | Label | IP / SteamID | Duration | Description
--- | :---: | :---: | :---: | ---
ATBAN | Hubert | 7656119796028xxxx | 60 | Bans the steamID provided for 60 minutes (1 hour)
OBAN | Gaben | 89.82.35.149 | 120 | Bans the IP provided for 120 minutes (2 hours)

### Note

[csharp]: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/built-in-types-table
[player]: https://github.com/Rnen/AdminToolbox/wiki/AdminToolbox.Commands.Variables
[resources]: (./Resources)
