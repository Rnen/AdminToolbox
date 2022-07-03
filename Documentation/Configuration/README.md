
# Configuration

*Optional parameter you can pass.*

<br>

## ![Label Damage]

[<kbd> <br> Tutorial <br> </kbd>][Damage Tutorial]
Choose what type of damage affect players with the `TUTORIAL` role.

[<kbd> <br> Global <br> </kbd>][Damage Tutorial]
Adjust the global damage multiplier.

<br>
<br>


### *Note that all configs should go in your server config file, not config_remoteadmin.txt

### Examples

```yaml
#The example would make scientists (6) not able to damage any class, dboys (1) not able to attack other dboys (1))
admintoolbox_block_role_damage: 6:0-1-2-3-4-5-6-7-8-9-10-11-12-13-14-15-16-17,1:1


admintoolbox_intercom_whitelist: owner:120:10, moderator:90:20
```

>Find a complete list of DamageID's, RoleID's and more in the [RESOURCES](Resources.md) page!



[Damage Tutorial]: Settings/Damage/Tutorial.md

[Label Damage]: https://img.shields.io/badge/Damage-C9284D?style=for-the-badge&logoColor=white&logo=ActiGraph