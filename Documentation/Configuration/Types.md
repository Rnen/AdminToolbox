
# Types

*Types and variables you can use in the settings.*

<br>

## Boolean

Either `true` or `false`.

<br>
<br>

## Integer

Any whole number / non-decimal.

`1` , `2` , `69` , `621`

<br>
<br>

## Float

A decimal number.

`0.9` , `22.2222`

<br>
<br>

## List

A list of items, separated by a comma.

`1,2,20,9,33`

<br>
<br>

## Dictionary

Key-Value pairs separated by commas.

`1:2` , `1:2,2:3,40:1`

<br>
<br>

## Random

If a config option has an `R` before it, it <br>
indicates you can use a random value.

<br>

- A random value is defined by having `{}`.
    
- Items are listed like `weight%value`.

- Weight values default to `1`.

- Pairs are separated by `|`.

<br>

### Example

```yaml
RandomList : {1%1|2%7|6},3,6,{15%3|2|45%2}
```

<br>
<br>

## SteamID64

A steam **[Profile Id]**.

<br>
<br>

## Variables

The following variables can be used in intercom messages:

- `$playerhp` | `$playerhealth`

- `$playerrole`

- `$playerteam`

- `$playerrank`

- `$playerid`

- `$player`

<br>


<!----------------------------------------------------------------------------->

[Profile Id]: https://steamid.xyz/