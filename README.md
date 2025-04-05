# Suikoden 2 Character Mod

A BepInEx plugin to export/import character data for suikoden 2 hd remaster

## Features

Exports the following character data into an editable json format:
- Stat growth rates
- Rune Affinities
- Rune Slot Levels

A config file is also available to enable the following:
- Force re-export character json
- Helper method to max stats for all characters

## Installation

- Download BepInEx 6.0.0-pre.2 IL2CPP build from [here](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip) and extract the content to the game directory (where `Suikoden I and II HD Remaster.exe` is located). Replace the files if asked.
- Download the mod and extract the content to `(GAME_PATH)\BepInEx\plugins\`. Replace the files if asked.
- Run the game once and enter the Suikoden 2 start menu to generate the config file, change the config in `(GAME_PATH)\BepInEx\config\s2_character_mod\characters.json` and restart the game.
- There is also a general config file in `(GAME_PATH)\BepInEx\config\s2_character_mod.cfg`

## Extra

Data about stat rates comes from [here](https://suikosource.com/phpBB3/viewtopic.php?t=14604&sid=dbfdb0826ac3d18eb4683ad8a13823f4). A PSX version of this mod also exists, created by me, hosted at 
[here](https://suikosource.com/phpBB3/viewtopic.php?p=160107&sid=d2c0f910b6b917af03f8dab4236da87a#p160107)