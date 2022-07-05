
# GhostMode

<kbd>  GH  </kbd>   or   <kbd>  GHOST  </kbd>   or   <kbd>  GHOSTM  </kbd>   or   <kbd>  GHOSTMODE  </kbd>

<br>
<br>

## Description

*Makes the given player invisible to other.*

<br>
<br>

## Enabling

*To use GhostMode, you will need to set the **[SMod]** <br>
config option  `sm_enable_ghostmode`  to  `true`.*

<br>
<br>

## Invisibility

*There are multiple invisibility types.*

<kbd>  State  </kbd>  <kbd>  Spectators  </kbd>  <kbd>  Speaking  </kbd>

<br>

### State

Is GhostMode enabled?

<br>

### Spectators

Can spectators see the ghost?

*Defaults to  `true`.*

<br>

### Speaking

Can other see a talking ghost?

*Requires the ghost to equip a radio.*

*Defaults to  `true`.*

<br>
<br>

## Listing

*To list all ghosts, pass  `LIST`  or  `GET`.*

<br>
<br>

## Examples

*Makes  `Evan`  a ghost.*

```shell
GHOST Evan true
```

<br>

*Makes  `Hubert`  a ghost that spectators cannot see.*

```shell
GHOST Hubert true false
```

<br>

*Makes  `Mike`  a ghost that cannot* <br>
*be seen while talking on the radio.*

```shell
GHOST Mike true true false
```

<br>

*Makes **everyone** a ghost.*

```shell
GHOST * true
```

<br>

*List whose a ghost.*

```shell
GHOST LIST
```

<br>


<!----------------------------------------------------------------------------->

[SMod]: https://github.com/ServerMod/Smod2
