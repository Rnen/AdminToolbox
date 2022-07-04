
# Configuration

*Optional parameters you can pass.*

<br>

## Note

Place all the following config options in your <br>
server config, not `config_remoteadmin.txt`.

<br>
<br>

## Types & Variables

Before you configure any setting, <br>
check the **[Types]** documentation.

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

<!----------------------------------------------------------------------------->

[Types]: Types.md


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

[Intercom Transmission]: Settings/Intercom/Transmission.md
[Intercom Blacklist]: Settings/Intercom/Blacklist.md
[Intercom Whitelist]: Settings/Intercom/Whitelist.md
[Intercom Limited]: Settings/Intercom/Limited.md
[Intercom Restart]: Settings/Intercom/Restart.md
[Intercom Ready]: Settings/Intercom/Ready.md

[Logging Housekeeping]: Settings/Logging/Housekeeping.md
[Logging Teamkill]: Settings/Logging/TeamKill.md
[Logging Commands]: Settings/Logging/Commands.md
[Logging Location]: Settings/Logging/Location.md
[Logging Unified]: Settings/Logging/Unified.md
[Logging Damage]: Settings/Logging/Damage.md
[Logging Kills]: Settings/Logging/Kills.md


<!--------------------------------[ Labels ]----------------------------------->

[Label Intercom]: https://img.shields.io/badge/Intercom-F47D31?style=for-the-badge&logoColor=white&logo=RSS
[Label Logging]: https://img.shields.io/badge/Logging-5C1F87?style=for-the-badge&logoColor=white&logo=AzureArtifacts
[Label General]: https://img.shields.io/badge/General-1A70B8?style=for-the-badge&logoColor=white&logo=Diaspora
[Label Damage]: https://img.shields.io/badge/Damage-C9284D?style=for-the-badge&logoColor=white&logo=ActiGraph
[Label Debug]: https://img.shields.io/badge/Debug-009287?style=for-the-badge&logoColor=white&logo=HubSpot
[Label Info]: https://img.shields.io/badge/Information-666666?style=for-the-badge&logoColor=white&logo=InternetArchive
