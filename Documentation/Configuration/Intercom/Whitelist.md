    
# Whitelist

<kbd>  admintoolbox_intercom_whitelist  </kbd>  
<kbd>  ServerRole:SpeakTime:CooldownTime  </kbd>

<br>
<br>

## Description

*Whitelist of server roles with speak limit and cooldown.*

<br>

### Roles

The usable roles can be found in <br>
the  `config_remoteadmin.txt`  file.

<br>

### Time

Time is specified in seconds.

<kbd>  1  </kbd>   **=̂**   <kbd>  1 Second  </kbd>

<br>
<br>

## Example

- `Owner`  can talk for  `120 Seconds`  every  `10 Seconds`.

- `Moderator`  can talk for  `90 Seconds`  every  `20 Seconds`.

```yaml
admintoolbox_intercom_whitelist : owner:120:10, moderator:90:20
````

<br>
