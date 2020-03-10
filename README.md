# Sonic4Ep1-WP7-Decompilation
Decompilation of Sonic 4 Episode 1 for Windows Phone 7, and subsequent port to Windows, macOS, Linux, Windows Phone, iOS and Android

# Features
 - Basic controller/keyboard support in the game itself (menus still depend on touch/mouse)
 - Toggleable loop camera [🦀🦀](https://twitter.com/da_wamwoowam/status/1236706830962905089)
 - Discord rich presence
 - Accelerometer support on iOS and Windows Phone (emulation soon)
 - 60 FPS support
 - Vastly better loading times than literally any other version of the game

# Issues

## Game is unbeatable
After fixing the major collision issues the game is *mostly* playable, however as it stands it's not possible to complete Casino Street Act 3 or Lost Labyrinth Act 2 due to other collision issues.

## AppMain.cs is still way too big
It's so big that it VS to lag pretty badly, but I've never had it hard crash.

## XNB files are perfectly fine
The issue is the format of the audio itself, MonoGame on Windows excpects OGG Vorbis audio, while the original audio files are WMAs. A simple conversion with ffmpeg and a mass rename is enough to fix most of these issues. I've also built a tool that changes the file extension the XNBs are looking for, to make this process easier.
