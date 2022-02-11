# Randomizer Map S
Randomizer Map S is a Hollow Knight mod used with Randomizer 4. It helps to find both item and transition checks.

![Example Screenshot](./worldmap.jpg)
![Example Screenshot](./transition.jpg)
![Example Screenshot](./pause.jpg)

This fork of CaptainDapper's original mod has been expanded on with more features, bug fixes and a Pause Menu UI. It is currently compatible with:
- RandomizerMod v4.0.2
- AdditionalMaps v1.5.1.0. This mod is optional, however *highly* recommended to see rooms/locations in White Palace
- RandomizableLevers v1.1.1.0
- ItemSync v2.2.0
- RandoPlus v1.1.0
- SkillUpgrades v0.9.4.1

https://github.com/homothetyhk/RandomizerMod

https://github.com/SFGrenade/AdditionalMaps

https://github.com/flibber-hk/HollowKnight.RandomizableLevers

https://github.com/Shadudev/HollowKnight.MultiWorld/tree/itemsync

https://github.com/flibber-hk/HollowKnight.RandoPlus

https://github.com/flibber-hk/HollowKnight.SkillUpgrades

# Quick Start Guide
- Press `CTRL-M` during a game to enable the mod. Alternatively, enable it from the Pause Menu.

# Features
- The World Map will now show Pins for every item check.
    - Big Pins means the items are reachable according to RandomizerMod's logic
    - Small Pins means the items are not randomized or not reachable
    - Pins will disappear as you check their locations
    - MapMod S settings are displayed at the bottom

- New to MapModS for Rando 4:
    - Previewed items will appear as pins with a green border
    - Persistent items will appear as pins with a cyan border and remain visible after being checked
    - Out-of-logic items will appear as pins with a red border

- The Pause Menu UI has the following buttons:
    - "Mod Enabled/Disabled": Toggle the mod on/off
    - "Spoilers" `CTRL-1`: Toggle Pins between vanilla (non-spoiler) and spoiler item pools
    - "Randomized" `Ctrl-2`: Toggle all Pins for randomized items on/off
    - "Others" `Ctrl-3`: Toggle all Pins for non-randomized items on/off
    - "Pin Style" `CTRL-4`: Toggle the style of the Pins
    - "Pin Size" `CTRL-5`: Toggle the size of the Pins
    - "Mode": Toggle the map mode
    - "Customize Pins": Open/close a panel with a toggle for each spoiler item pool

- The mod currently has four main modes:
   - "Transition": For transition rando runs only
   - "Transition 2": For area rando runs only. Instead of showing all in-logic + visited rooms, only visited rooms appear
   - "Full Map": Shows all pins and the full map regardless of map items obtained
   - "All Pins": Shows all pins, but only show map areas for obtained map items
   - "Pins Over Map": Only show pins over where the corresponding map item has been obtained

- "Transition" mode is a new mode in MapModS for Rando 4 that displays visited rooms with color-coding:
    - Current room is green
    - Adjacent visited rooms are cyan
    - Rooms containing unchecked reachable transitions are brighter
    - Out of logic/sequence break rooms are red
    - Other visited rooms are a standard grey/white
 
- In "Transition" mode, you also have a route searcher in the World Map that allows you to find a sequence of transitions to get to any other selected room on the map.
    - Hover over the desired room (in yellow) and press your bound `Menu Select` button to attempt a route search.
    - If successful, the route will be displayed.
    - You can toggle including benchwarp in the route search on/off with `CTRL-B`.
    - You can try again (pressing the button) to change the start/final transition.
    - The route will also be visible from the Quick Map for easy access, and gets updated as you make the suggested transitions.
    - The World Map can list unchecked and visited transitions for any selected room, toggled with `CTRL-U`.
    - The Quick Map lists unchecked and visited transitions for the room you are currently in.

# How To Install
Use Scarab: https://github.com/fifty-six/Scarab

Or, you can install manually:
1. Download the latest release of `MapModS.zip`.
2. Unzip and copy the folder 'MapModS' to `...\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods`.

# Acknowledgements
- Special thanks to Homothety and Flib for significant coding help
- CaptainDapper for making the original mod
- PimpasPimpinela for helping to port the mod from Rando 3 to Rando 4
- Chaktis, KingKiller39 and Ender Onryo for helping with sprite art
- ColetteMSLP for testing out the mod during livestreams
