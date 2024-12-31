# Введение
<image src="Eclair/Assets/icon.png" width="300"/>
<image src="res/screenshot.png"/>

**Eclair** - это open-source аудиоплеер написанный на *C#*.
Этот плеер постепенно развивается и нацелен не только на прослушивания музыки, но и на красоту и удобство использования.<br>
Я буду рад любой помощи в развитии проекта!

## Почему Eclair open-source?
**Eclair** это моё хобби, как и программирование в целом.
Я поддерживаю свободное ПО, и именно поэтому **Eclair** open-source.
Я желаю делиться своим исходным кодом проекта.
Мне нравиться разрабатывать этот проект и я не считаю что этот плеер должен быть платным и с закрытым исходным кодом.<br>
Да и скорее всего никто бы не стал покупать аудио плеер, так ещё от кого-то [*noname*](https://github.com/NonExistPlayer).

## Поддерживаемые аудио-форматы
| Формат | Название                                                |
| :----: | :------------------------------------------------------ |
| `.mp3` | *MPEG-1 Audio Layer III*                                |
| `.aac` | *Advanced Audio Coding*                                 |
| `.asf` | *Active Streaming Format или Advanced Streaming Format* |
| `.wma` | *Windows Media Audio*                                   |
| `.ogg` | *Ogg*                                                   |
| `.flac`| *Free Lossless Audio Codec*                             |
| `.midi`| *MIDI*                                                  |

## Поддерживаемые ОС
| Система  | Минимальная версия |
| :------- | :----------------: |
| Windows  | 8.0                |
| Debian   | 9                  |
| Ubuntu   | 16.04              |
| Fedora   | 30                 |
| Android  | 8.0                |
| ~~iOS~~  | -                  |
| ~~MacOS~~| -                  |

Почему `iOS` и `MacOS` не поддерживаются? Прочитайте [этот документ](res/about_apple_devices.md).

<!--
# Установка
## Установка на Windows
## Установка на Linux
Перед установкой архива программы, установите следующие пакеты командой:
```bash
sudo apt install vlc libvlc-dev libvlccore-dev
```
## Установка на Android
> [!CAUTION]
> При удалении данных приложения скорее всего приложение больше не запустится!
> Нужно будет переустановить приложение чтобы, оно продолжило работать.
-->

# Использованые...
## Библиотеки
* [Avalonia](https://github.com/AvaloniaUI/Avalonia)
    - [Avalonia.Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia) (11.0.0)
    - [MessageBox.Avalonia](https://github.com/AvaloniaCommunity/MessageBox.Avalonia) (3.1.0)
* [LibVLCSharp](https://github.com/videolan/libvlcsharp)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json/)
* [TagLibSharp](https://github.com/mono/taglib-sharp)
* [NonExistPlayer/Logging](https://github.com/NonExistPlayer/Logging)
## Сайты
[Иконки](https://icon-icons.com)
[Нейросеть](https://blackbox.ai)
## Устройства

### Windows
**Система**: **Microsoft Windows 11 Pro** *10.0.22621 сборка 22621*
### Linux
**Система**: **Ubuntu 24.04**
### Android
|Устройство|Система|Номер сборки|
|:-:|:-:|:-:|
|[Xiaomi POCO M3 Pro 5G](https://www.google.com/search?q=Xiaomi+POCO+M3+Pro+5g)|Android **13** (**API 33**)|*TQ2A.230505.002 PixelExperience_Plus_camellia-13.0-20230610-2200-UNOFFICIAL*|
|[Google Pixel 3 XL](https://www.google.com/search?q=Google+Pixel+3+xl)|Android **12** (**API 31**)|*SP1A.210812.016.C1*|

# [Лицензия](LICENSE)
**Eclair** защищён лицензией [MIT](https://choosealicense.com/licenses/mit). Просмотрите файл [LICENSE](LICENSE).