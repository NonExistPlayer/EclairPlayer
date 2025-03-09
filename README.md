# Introduction
<image src="Eclair/Assets/icon.png" width="300" alt="Icon" style="margin-right: 20px;"/>
<image src="res/screenshot.png"/>

**Eclair** is an open-source audio player written in *C#*.
This player is gradually developing and is aimed not only at listening to music, but also at beauty and ease of use.<br>
I will be glad to any help in developing the project!

## BASS
**Eclair** uses the BASS audio library for audio playback.
> BASS Audio Library (https://www.un4seen.com)<br>
> Copyright (c) 1999-2023 Un4seen Developments Ltd.

> BASS is used under the license granted by Un4seen Developments.<br>
> For more information, visit https://www.un4seen.com.

## Why is Eclair open-source?
**Eclair** is my hobby, as is programming in general.
I support free software, and that is why **Eclair** is open-source.
I want to share my project source code.
I like developing this project and I do not think that this player should be paid and with closed source code.<br>
And most likely no one would buy an audio player, especially from [*noname*](https://github.com/NonExistPlayer).

## Supported audio formats
| Format | Name |
| :----: | :------------------------------------------------------ |
| `.mp3` | *MPEG-1 Audio Layer III* |
| `.aac` | *Advanced Audio Coding* |
| `.asf` | *Active Streaming Format or Advanced Streaming Format* |
| `.wma` | *Windows Media Audio* |
| `.ogg` | *Ogg* |
| `.flac`| *Free Lossless Audio Codec* |
| `.midi`| *MIDI* |

## Supported OS
| System | Minimum Version |
| :------- | :----------------: |
| Windows | 8.0 |
| Debian | 9 |
| Ubuntu | 16.04 |
| Fedora | 30 |
| Android | 8.0 |
| ~~iOS~~ | - |
| ~~MacOS~~| - |

Why aren't `iOS` and `MacOS` supported? Read [this document](res/apple.md).

<!--
# Installation
## Installation on Windows
## Installation on Linux
Before installing the program archive, install the following packages with the command:
```bash
sudo apt install vlc libvlc-dev libvlccore-dev
```
## Installation on Android
> [!CAUTION]
> If you delete the application data, the application will most likely not start anymore!
> You will need to reinstall the application so that it continues to work.
-->

# Used...
## Libraries
* [Avalonia](https://github.com/AvaloniaUI/Avalonia)
    - [Avalonia.Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia) (11.0.0)
    - [MessageBox.Avalonia](https://github.com/AvaloniaCommunity/MessageBox.Avalonia) (3.1.0)
* [LibVLCSharp](https://github.com/videolan/libvlcsharp)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json/)
* [TagLibSharp](https://github.com/mono/taglib-sharp)
* [NonExistPlayer/Logging](https://github.com/NonExistPlayer/Logging)
## Sites
[Icons](https://icon-icons.com)<br>
[AI](https://blackbox.ai)
## Devices
### Windows
**System**: **Microsoft Windows 10 Enterprise LTSC** *(x86 VirtualBox Machine)*
### Linux
**System**: **Manjaro Linux** *6.12.4-1-MANJARO*
### Android
|Device|System|Build number|
|:-:|:-:|:-:|
|Google Pixel 6 Pro|Android **15** (**API 35**)|*AP4A.250105.002*|
|Google Pixel 3 XL|Android **12** (**API 31**)|*SP1A.210812.016.C1*|

# [List of tasks](res/todo.md)
You can view the list of project tasks by clicking on the link above.

# [License](LICENSE)
**Eclair** is licensed under the [MIT](https://choosealicense.com/licenses/mit). Please review the [LICENSE](LICENSE) file.