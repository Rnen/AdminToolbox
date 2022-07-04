
# Configuration

*Optional parameter you can pass.*

<br>

## ![Label Damage]

[<kbd> <br>          Tutorial          <br> </kbd>][Damage Tutorial]   
Choose what type of damage affect players with the `TUTORIAL` role.

[<kbd> <br>             Global             <br> </kbd>][Damage Tutorial]   
Adjust the global damage multiplier.

[<kbd> <br>         After Party         <br> </kbd>][Damage After Party]   
Adjust the damage that is dealt after the round ends.

[<kbd> <br> LCZ De-Containment <br> </kbd>][Damage LCZ]   
Adjust the LCZ De-Containment damage multiplier.

[<kbd> <br>           Blocked           <br> </kbd>][Damage Blocked]   
Disable specific roles from damaging each other.

<br>
<br>

## ![Label Info]

[<kbd> <br> Join <br> </kbd>][Info Join]   
Set if player names should be shown in the server console upon joining.

<br>
<br>

## ![Label Debug]

[<kbd> <br>          Damage Detection          <br> </kbd>][Debug Detected]   
Control what damage is detected.

[<kbd> <br>             Enemy Damage             <br> </kbd>][Debug Enemy]   
Display damage done to enemy players.

[<kbd> <br>        Friendly Damage         <br> </kbd>][Debug Friendly]   
Display damage done to friendly players.

[<kbd> <br> Enemy Kills <br> </kbd>][Debug Kills]   
Display enemy kills in the server console.

[<kbd> <br>           Special Kills           <br> </kbd>][Debug Special]   
Display suicides, scp kills & grenade kills.

[<kbd> <br>           Team Kills           <br> </kbd>][Debug Teamkill]   
Disable team kills in the server console.

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



[Damage After Party]: Settings/Damage/After%20Party.md
[Damage Tutorial]: Settings/Damage/Tutorial.md
[Damage Blocked]: Settings/Damage/Blocked.md
[Damage Global]: Settings/Damage/Global.md
[Damage LCZ]: Settings/Damage/LCZ%20De-Containment.md

[Info Join]: Settings/Info/Join.md

[Debug Teamkill]: Settings/Debug/Teamkill.md
[Debug Detected]: Settings/Debug/Detected.md
[Debug Friendly]: Settings/Debug/Friendly.md
[Debug Special]: Settings/Debug/Special.md
[Debug Kills]: Settings/Debug/Kills.md
[Debug Enemy]: Settings/Debug/Enemy.md


[Label Damage]: https://img.shields.io/badge/Damage-C9284D?style=for-the-badge&logoColor=white&logo=ActiGraph
[Label Debug]: https://img.shields.io/badge/Debug-009287?style=for-the-badge&logoColor=white&logo=HubSpot
[Label Info]: https://img.shields.io/badge/Information-666666?style=for-the-badge&logoColor=white&logo=InternetArchive
