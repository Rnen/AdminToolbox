
# Damage Settings

<br>

## Tutorial Damage

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
<br>



### Damage Settings

Damage Config Option | Value Type | Default Value | Description
--- | :--- | :--- | ---:
admintoolbox_round_damageMultiplier | Float | 1 | Multiplies all damage by this number
admintoolbox_endedRound_damageMultiplier | Float | 1 | Multiplies all damage by this number after round ends. For maximum chaos enter high number (10 or something) To turn off dmg on round end, enter `0`.
admintoolbox_decontamination_damagemultiplier | Float | 1 | Multiplies LCZ decontaimnent damage with the specified number
admintoolbox_block_role_damage | roleX:role1 |  |  Blocks damage between specified roles.
***

```yaml
#Example: Chaos cannot damage Dboys, scientists cannot harm any MTF. (But opposite is fine for both)
admintoolbox_block_role_damage: 8:1,6:12:13:15
```
