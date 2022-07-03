
# Damage Settings

<br>

## Tutorial

<kbd>  admintoolbox_tutorial_dmg_allowed  </kbd>  
<kbd>  List  ➞  -1  </kbd>

<br>

### Description

*Sets what types of damage the  `TUTORIAL`  role can take.*

<br>

### Choices

#### None    <kbd>  -1  </kbd>

*No damage will be taken.*

#### All    <kbd>  -2  </kbd>  <kbd>  *  </kbd>  <kbd>  all  </kbd>

*Any type of damage will be taken.*

<br>

### Example

```yml
admintoolbox_tutorial_dmg_allowed : all
```

<br>

<!----------------------------------------------------------------------------->

<br>

## Global

<kbd>  admintoolbox_round_damageMultiplier  </kbd>  
<kbd>  Float  ➞  1  </kbd>

<br>

### Description

*Global damage multiplier.*

<br>

### Example

```yml
admintoolbox_round_damageMultiplier : 6.9
```

<br>

<!----------------------------------------------------------------------------->

<br>

## After Party

<kbd>  admintoolbox_endedRound_damageMultiplier  </kbd>  
<kbd>  Float  ➞  1  </kbd>

<br>

### Description

*Global damage multiplier for after the round ends.*

<br>

### Example

```yml
admintoolbox_endedRound_damageMultiplier :  0 # No fun
```

```yml
admintoolbox_endedRound_damageMultiplier : 10 # Chaos
```

<br>

<!----------------------------------------------------------------------------->

<br>

## LCZ De-Containment

<kbd>  admintoolbox_decontamination_damagemultiplier  </kbd>  
<kbd>  Float  ➞  1  </kbd>

<br>

### Description

*LCZ de-containment damage multiplier.*

<br>

### Example

```yml
admintoolbox_decontamination_damagemultiplier : 4.2
```

<br>

<!----------------------------------------------------------------------------->

<br>

## LCZ De-Containment

<kbd>  admintoolbox_block_role_damage  </kbd>  
<kbd>  RoleA:RoleB  </kbd>

<br>

### Description

*Blocks `RoleA` from damaging `RoleB`.*

<br>

### Example

- `Chaos` cannot damage `DBoys`
- `Scientists` cannot harm `MTF`
- The opposite is not blocked.

```yml
admintoolbox_block_role_damage : 8:1,6:12:13:15
```

<br>