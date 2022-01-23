# News Helicopter
Remember the news helicopters that spawned along with the police helicopters in GTA San Andreas? Me too. With this version, you can get access to the live camera feed from the news chopper.

---
## Installation
Move `NewsHeli.dll` and `NewsHeli.ini` to your `/scripts` directory

### Requirements
- ScriptHookV
- ScriptHookVDotNet 3
- .NET Runtime 4.8
- **recommended**: a helicopter with a news station livery, like [this one](https://www.gta5-mods.com/vehicles/buckingham-maverick-2nd-generation-add-on-liveries)

---
## Usage
The news heli will spawn automatically when you have 3 or more stars wanted level.  

### Keyboard
Press `[Enter]` (can be changed in INI) to toggle the news chopper camera feed.  
Press `[Ctrl]` and `[+]` or `[-]` to zoom camera feed in/out, respectively. Same as zooming a web browser.

### Gamepad
*Note: these control combinations also apply to keyboard. They must be pressed in order (the same way you copy with Ctrl+C).*

Function | Control | Default
---|---|---
toggle camera | `CharacterWheel` + `LookBehind` | `[DPad-down]` + `[RightStick-click]`
zoom in | `LookUp` + `LookBehind` | `[RightStick-Up]` + `[RightStick-click]`
zoom out | `LookDown` + `LookBehind` | `[RightStick-Down]` + `[RightStick-click]`


### Settings
Setting | Function | Default
---|---|---
**NewsHeli**|
`onWanted` | spawn news chopper with 3 or more wanted stars? Currently there is no way to spawn one manually, so leave as `true` | `true`
`model` | helicopter model to be spawned. You can use add-on model names | `frogger`
`radius` | how far away the helicopter should follow the player from, in meters | `40` meters
`altitude` | how high the helicopter should be above the player, in meters | `40` meters
`respawnDelay` | time delay (in seconds) before the helicopter spawns again | `30` seconds
`minWantedLevel` | wanted level at which news chopper spawns | `2`
**HeliCam**|
`activateKey` | Key used to toggle the news chopper camera feed | `Enter` (return carriage)
`defaultFov` | Default field-of-view for the news chopper camera feed, in degrees | `35` degrees
`zoomFactor` | Percentage to zoom camera by every time you press the zoom command | `20` %
`showWeazelOverlay` | If true, show Weazel News overlay when news chopper camera feed is active | `true`
`title` | Title to display on news overlay. If left blank, "Live Breaking News" is shown |
`subtitle` | Subtitle to display on news overlay. If left blank, a subtitle is randomly chosen | 

---
## Development
[Source code - GitHub](https://github.com/DavidLiuGit/GTAV_NewsHelicopter)  
[GTA5 Mods](https://www.gta5-mods.com/scripts/news-helicopter)

### Planned features
- ability to apply filters to camera feed
- gracefully transition from gameplay cam to camera feed
- community suggestions!

### Changelog
#### 1.5.3
- more headlines
#### 1.5.2
- added option to change wanted level needed for news heli to spawn
#### 1.5.1
- Weazel News overlay will now show "Signal Lost" if heli is destroyed
#### 1.5.0
- implemented Weazel News overlay
- modified some default settings (FOV and zoom factor)
#### 1.4.0
- heli will now spawn further away and fly in
#### 1.3.0
- implemented respawn delay; default 30 seconds
#### 1.2.1
- updated error checking
- reverted pilot to reporter only (no more Beverly)
#### 1.2.0
- implemented gamepad controls. See usage section for details
- decreased pilot retasking rate to once per 5 seconds to prevent erratic flying
#### 1.1.1
- added error checking after spawning heli & crew
- fixed a bug where the camera would not zoom after the heli is destroyed in rare cases
#### 1.1
- heli pilot now flies away when dismissed
- implemented zoom in/out heli camera feed
- increased script tick rate to make it more responsive
#### 1.0.1
- shifted the camera mounting position further forward
- heli now dismissed if player loses wanted level
#### 1.0
- initial release