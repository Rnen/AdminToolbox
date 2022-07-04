
# Configuration

*Optional parameter you can pass.*

<br>

## ![Label Logging]

[<kbd> <br>         Commands        <br> </kbd>][Logging Commands]   
Log command usage.

[<kbd> <br>          Damage          <br> </kbd>][Logging Damage]   
Log player damage.

[<kbd> <br>     Housekeeping     <br> </kbd>][Logging Housekeeping]   
Remove logs after a duration.

[<kbd> <br>           Kills           <br> </kbd>][Logging Kills]   
Remove enemy kills.

[<kbd> <br>  Plugin Location  <br> </kbd>][Logging Location]   
Plugin folder location.

[<kbd> <br>        Teamkills        <br> </kbd>][Logging Teamkill]   
Log team kills.

[<kbd> <br>     Unified Data     <br> </kbd>][Logging Unified]   
Combine data from different servers.

<br>
<br>

## ![Label Intercom]

[<kbd> <br>            Blacklist            <br> </kbd>][Intercom Blacklist]   
Block players from using the intercom system.

[<kbd> <br>        Limited Access        <br> </kbd>][Intercom Limited]   
Only allow whitelisted players.

[<kbd> <br>         Ready Message         <br> </kbd>][Intercom Ready]   
Message shown when intercom is ready.

[<kbd> <br>       Restart Message       <br> </kbd>][Intercom Restart]   
Message shown when intercom is restarting.

[<kbd> <br>  Transmission Message  <br> </kbd>][Intercom Transmission]   
Message shown when intercom is transmitting.

[<kbd> <br>            Whitelist            <br> </kbd>][Intercom Whitelist]   
Allow specific roles to use the intercom.

<br>
<br>

## ![Label General]

[<kbd> <br>  Colorized Console  <br> </kbd>][General Color]   
Colorize the console output.

[<kbd> <br>      Enable Plugin      <br> </kbd>][General Enable]   
Enable this plugin.

[<kbd> <br>   Join Information   <br> </kbd>][General Info]   
Show extended info for joining players.

[<kbd> <br>          Join Name           <br> </kbd>][General Name]   
Show joining players names.

[<kbd> <br>   Match Statistics   <br> </kbd>][General Match]   
Display match Statistics.

[<kbd> <br>    Server Tracking    <br> </kbd>][General Tracking]   
Add tracking in the server name.

[<kbd> <br>   Version Checking   <br> </kbd>][General Version]   
Check for new versions of **AdminToolbox**.

<br>
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

[<kbd> <br>               Join               <br> </kbd>][Info Join]   
Set if player names should be shown in the server console upon joining.

<br>
<br>

## ![Label Debug]

[<kbd> <br>  Damage Detection  <br> </kbd>][Debug Detected]   
Control what damage is detected.

[<kbd> <br>      Enemy Damage      <br> </kbd>][Debug Enemy]   
Display damage done to enemy players.

[<kbd> <br>   Friendly Damage   <br> </kbd>][Debug Friendly]   
Display damage done to friendly players.

[<kbd> <br>         Enemy Kills         <br> </kbd>][Debug Kills]   
Display enemy kills in the server console.

[<kbd> <br>       Special Kills       <br> </kbd>][Debug Special]   
Display suicides, scp kills & grenade kills.

[<kbd> <br>          Team Kills          <br> </kbd>][Debug Teamkill]   
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



<!-------------------------------[ Settings ]---------------------------------->

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

[General Tracking]: Settings/General/Tracking.md
[General Version]: Settings/General/Version.md
[General Enable]: Settings/General/Enable.md
[General Color]: Settings/General/Color.md
[General Match]: Settings/General/Match.md
[General Name]: Settings/General/Joined.md
[General Info]: Settings/General/Information.md

[Intercom Transmission]: Settings/General/Transmission.md
[Intercom Blacklist]: Settings/General/Blacklist.md
[Intercom Whitelist]: Settings/General/Whitelist.md
[Intercom Limited]: Settings/General/Limited.md
[Intercom Restart]: Settings/General/Restart.md
[Intercom Ready]: Settings/General/Ready.md

[Logging Housekeeping]: Settings/General/Housekeeping.md
[Logging Teamkill]: Settings/General/TeamKill.md
[Logging Commands]: Settings/General/Commands.md
[Logging Location]: Settings/General/Location.md
[Logging Unified]: Settings/General/Unified.md
[Logging Damage]: Settings/General/Damage.md
[Logging Kills]: Settings/General/Kills.md


<!--------------------------------[ Labels ]----------------------------------->

[Label Intercom]: https://img.shields.io/badge/Intercom-F47D31?style=for-the-badge&logoColor=white&logo=RSS
[Label Logging]: https://img.shields.io/badge/Logging-5C1F87?style=for-the-badge&logoColor=white&logo=AzureArtifacts
[Label General]: https://img.shields.io/badge/General-1A70B8?style=for-the-badge&logoColor=white&logo=Diaspora
[Label Damage]: https://img.shields.io/badge/Damage-C9284D?style=for-the-badge&logoColor=white&logo=ActiGraph
[Label Debug]: https://img.shields.io/badge/Debug-009287?style=for-the-badge&logoColor=white&logo=HubSpot
[Label Info]: https://img.shields.io/badge/Information-666666?style=for-the-badge&logoColor=white&logo=InternetArchive
