# Empty

Empties the inventory of the spesified player. Optionally deletes the items completely.

## Command usage

* EMPTY [PLAYER][player] <ITEM-ID/DELETE> <DELETE>

Parameter | Default State | Description
--: | :--: | :--
ITEM-ID | | See [resource][resources] page

### Command Aliases

List of Aliases for the word `EMPTY`

* E

## Examples

Command | [Player][player] | (Item-ID / DELETE) | <DELETE> | Description
--- | :---: | :---: | :---: | ---
EMPTY | Evan | 11 | | Drops all 05 keycards from player
EMPTY | Robert | 0 | DELETE | Permamently deletes all Janitor keycards from player
EMPTY | Frank | | | Drops all items from player (simmilar to "dropall")
EMPTY | Bob | DELETE | | Permamently deletes all items in Bob's inventory

### Note

List of Aliases for the word `DELETE`

* DEL
* D

[csharp]: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/built-in-types-table
[player]: ../Variables.md
[resources]: ../Resources.md