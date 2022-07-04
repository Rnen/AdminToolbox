
# Tutorial Damage

<kbd>  admintoolbox_tutorial_dmg_allowed  </kbd>  
<kbd>  List  ➞  -1  </kbd>

<br>
<br>

## Description

*Sets what types of damage the  `TUTORIAL`  role can take.*

<br>
<br>

## Choices

*Values you can use.*

<br>

### None

*No damage will be taken.*

```yaml
admintoolbox_tutorial_dmg_allowed : -1
```

<br>

### All

*Any type of damage will be taken.*

```yaml
admintoolbox_tutorial_dmg_allowed : all
```

```yaml
admintoolbox_tutorial_dmg_allowed : *
```

```yaml
admintoolbox_tutorial_dmg_allowed : -2
```

<br>

### Specific

*Allows damage from*:

- Poison ( **29** )

- Tesla ( **5** )

- P99 ( **13** )

```yaml
admintoolbox_tutorial_dmg_allowed : 29,13,5
```

<br>
