
# Blocked Damage

<kbd>  admintoolbox_block_role_damage  </kbd>  
<kbd>  RoleA:RoleB  </kbd>

<br>
<br>

## Description

*Blocks  `RoleA`  from damaging  `RoleB`.*

<br>
<br>

## Example

<br>

- `Chaos`  ( **8** ) cannot damage  `DBoys`  ( **1** )

- `Scientists`  ( **6** ) cannot harm  `MTF`

- The opposite is not blocked.

<br>

```yaml
admintoolbox_block_role_damage : 8:1,6:12:13:15
```

<br>

- `Scientists`  ( **6** ) cannot damage anyone

- `DBoys`  ( **1** ) cannot attack each other

```yaml
admintoolbox_block_role_damage : 6:0-1-2-3-4-5-6-7-8-9-10-11-12-13-14-15-16-17,1:1
```

<br>