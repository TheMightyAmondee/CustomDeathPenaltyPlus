
Custom Death Penalty Plus allows the death penalty (and pass out penalty as of 1.1.0) to be fully customised in [Stardew Valley](https://www.stardewvalley.net/).

Current options allow configuration for:

**Death Penalty**
- Disabling item loss
- Amount of money lost, including max amount that can be lost
- Amount of health and energy to restore
- A more realistic option for dying, disabled by default
- A friendship penalty with Harvey

**Pass Out Penalty**
- Amount of money lost, including max amount that can be lost
- Amount of energy to restore next day

### Installation:
1. Install the latest version of SMAPI
2. Install Custom Death Penalty Plus, available [here](https://www.nexusmods.com/stardewvalley/mods/7069).
3. Extract the contents into your Mods folder
4. Run the game through SMAPI

### Usage:
1. Run the game at least once to generate the config file
2. Change any desired values in the config. Percentage values are expressed in decimal form
3. Your changes should now be implemented in game

Note: The mod will create per-save file JSON files (found in the mod folder) so that the mod knows what to restore if you exit the next day without saving after dying or passing out. While deleting these will not affect the game, it can lead to incorrect values being used. Only delete the file if you are deleting the character.

### If changes are not implemented:
 - Check the mod page for accepted values
 - The mod should automatically use the default values if a config value is invalid. If your changes are not implemented check the SMAPI monitor for errors or messages.
### Debug Commands:
Version 1.3.0 introduced console commands to allow config values to be changed in the SMAPI console, mostly intended for debugging purposes. When entering commands all characters should be in lower case.

Available commands:

Command | Format | Action
------------ | -------------|----------------
deathpenalty | deathpenalty &lt;configvalue&gt; &lt;value&gt; | changes the deathpenalty config values
passoutpenalty | passoutpenalty &lt;configvalue&gt; &lt;value&gt; | changes the passoutpenalty config values
configinfo | configinfo | displays the current config settings in the SMAPI console

where config value is the name of the config option in the config file in lower case and the value is the value as it would appear in the config file.

The configvalue argument will also accept a shorthand version of the config value as shown in the table:

Config Value | Shorthand Value
------------ | ---------------
restoreitems | items
moneylosscap | cap
moneytorestorepercentage | money
healthtorestorepercentage | health
energytorestorepercentage | energy
wakeupnextdayinclinic | nextday
friendshippenalty | friendship
 
### Versions:
1.0.0 - Initial release

1.1.0 - Passing out penalty is now fully customisable. Added config option for a more realistic death. Minor bug fixes

1.2.0 - Added config option for a friendship penalty with Harvey. Removed items lost menu if items will be restored. Bug fixes

1.2.1 - Fully compatible with multiplayer

1.2.2 - Fixed bug where money would not restore correctly when passing out

1.3.0 - Updated for Stardew Valley 1.5, added debug commands


