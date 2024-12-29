# Introduction
<image src="Eclair/Assets/icon.png" width="300"/>
<image src="res/screenshot.png"/>

**Eclair** is an open-source audio player written in *C#*.
This player is gradually developing and is aimed not only at listening to music, but also at beauty and ease of use.<br>
I will be glad to any help in developing the project!
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

Why aren't `iOS` and `MacOS` supported? Read [this document](res/about_apple_devices.md).

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
[Icons](https://icon-icons.com)
[AI](https://blackbox.ai)
## Devices
### Windows
**System**: **Microsoft Windows 11 Pro** *10.0.22621 build 22621*
### Linux
**System**: **Ubuntu 24.04**
### Android
|Device|System|Build number|
|:-:|:-:|:-:|
|[Xiaomi POCO M3 Pro 5G](https://www.google.com/search?q=Xiaomi+POCO+M3+Pro+5g)|Android **13** (**API 33**)|*TQ2A.230505.002 PixelExperience_Plus_camellia-13.0-20230610-2200-UNOFFICIAL*|
|[Google Pixel 3 XL](https://www.google.com/search?q=Google+Pixel+3+xl)|Android **12** (**API 31**)|*SP1A.210812.016.C1*|

# [License](LICENSE)
**Eclair** is licensed under the [MIT](https://choosealicense.com/licenses/mit). Please review the [LICENSE](LICENSE) file.