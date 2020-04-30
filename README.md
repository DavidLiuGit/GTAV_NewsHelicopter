# News Helicopter

Remember the news helicopters that spawned along with the police helicopters in GTA San Andreas? Me too. With this version, you can get access to the live camera feed from the news chopper.


## Installation
Move `NewsHeli.dll` and `NewsHeli.ini` to your `/scripts` directory

### Requirements
- ScriptHookV
- ScriptHookVDotNet 3
- .NET Runtime 4.8
- **recommended**: a helicopter with a news station livery, like [this one](https://www.gta5-mods.com/vehicles/buckingham-maverick-2nd-generation-add-on-liveries)

## Usage
The news heli will spawn automatically when you have 3 or more stars wanted level.  
Press `[Enter]` (can be changed in INI) to toggle the news chopper camera feed.

### Settings
Setting | Function | Default
---|---|---
**NewsHeli**|
`onWanted` | spawn news chopper with 3 or more wanted stars? Currently there is no way to spawn one manually, so leave as `true` | `true`
`model` | helicopter model to be spawned. You can use add-on model names | `frogger`
`radius` | how far away the helicopter should follow the player from, in meters | `40` meters
`altitude` | how high the helicopter should be above the player, in meters | `40` meters
**HeliCam**|
`activateKey` | Key used to toggle the news chopper camera feed | `Enter` (return carriage)
`defaultFov` | Default field-of-view for the news chopper camera feed, in degrees | `60` degrees


## Development

[Source code - GitHub](https://github.com/DavidLiuGit/GTAV_NewsHelicopter)  
[GTA5 Mods](https://www.gta5-mods.com/scripts/news-helicopter)

### Planned features
- ability to zoom in/out on the camera feed
- ability to apply filters to camera feed
- gracefully transition from gameplay cam to camera feed
- community suggestions!

### Changelog
#### 1.0.1
- shifted the camera mounting position further forward
- heli now dismissed if player loses wanted level
#### 1.0
- initial release