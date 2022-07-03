# LOCKDOORS

Makes the user able to lock doors upon interaction

## Command usage

* LOCKDOORS <[Boolean][csharp]: State>
* LOCKDOORS <[PLAYER][player]> <[Boolean][csharp]: State>
* LOCKDOORS LIST

Parameter | Default State | Description
--: | :--: | :--
PLAYER | Self-Targeting | Unless other player is specified, targets self
State | Toggle | Unless specified, enabled state toggles

### Command Aliases

List of Aliases for the word `LOCKDOORS`

* DOORLOCK
* ATLOCK
* ATLD
* ATDL

## Examples

Command | Arguement_1 | Arguement_2 | Description
--- | :---: | :---: | ---
LOCKDOORS | * |  | Enables lockdoors for ALL players
ATLD | Evan | True | Enables lockdoors
ATLOCK | Hubert | 0 | Disables lockdoors 
ATDL | LIST | | Lists all players with LOCKDOORS enabled


[csharp]: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/built-in-types-table
[player]: https://github.com/Rnen/AdminToolbox/wiki/AdminToolbox.Commands.Variables
[resources]: https://github.com/Rnen/AdminToolbox/wiki/Resources