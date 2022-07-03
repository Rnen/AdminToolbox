# GhostMode

Command to go invisible for other players.

## Command usage

* GHOSTMODE <[Player][player]> <[Boolean][csharp]: State> <[Boolean][csharp]: Visible to Spectator> <[Boolean][csharp]: Visible when speaking>

Parameter | Default State | Description
--: | :--: | :--
PLAYER | Self-Targeting | Unless other player is specified, targets self
State | N/A | The ON/OFF state of the command
Visible to spectator | True | If spectator can see the user or not
Visible while speaking | True | If other players can see the user while user is speaking

> Note that "Visible while speaking" only works with a radio in the user's hand

### Command Aliases

List of Aliases for the word `GHOSTMODE`

* GH
* GHOST
* GHOSTM

## Examples

Command | [Player][player] | [Bool][csharp] (Default: Toggle) | [Bool][csharp] (Default: True) | [Bool][csharp] (Default: True) | Description
--- | :---: | :---: | :---: | :---: | ---
GHOST | Evan | true | | | This would enable ghostmode with `visible to spectator` & `visible when speaking` to true
GHOSTMODE | Hubert | true | false | | This would enable ghostmode without beeing visible to spectators
GH | * | true | | | Would enable ghostmode for all players
GHOSTM | LIST / GET | | | | Would list all players with GHOSTMODE active

### Note

All [bools][csharp] are optional
>The SMod `sm_enable_ghostmode` config needs to be set to `true` to use GHOSTMODE

[csharp]: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/built-in-types-table
[player]: ../Variables.md
[resources]: https://github.com/Rnen/AdminToolbox/wiki/Resources
