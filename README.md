# IMPORTANT
This is a `scratch` branch. In this branch, Eclair is rewritten **from scratch**.
## Why?
The code was written when I was just getting started with Avalonia
and so I made a lot of mistakes and strange decisions.<br>
That's why I decided to rewrite the project from scratch.
## Development
This branch is unstable and is not recommended for everyday use.<br>
The entire application and all decisions before the release are not final and can be changed.

# Introduction
**Eclair** is an open-source audio player written in
C# using Avalonia and Eclair.Core.
This player is gradually evolving and is aimed not only at listening to music, but also at beauty and ease of use.<br>
I will be glad to any help in developing the project!

## BASS
**Eclair** uses [its own wrapper](Eclair.Core) on top of the BASS library (ManagedBass)
for audio playback.
> BASS Audio Library (https://www.un4seen.com)<br>
> Copyright (c) 1999-2023 Un4seen Developments Ltd.

> BASS is used under license from Un4seen Developments.<br>
> For more information, visit https://www.un4seen.com.

## Why Eclair?
**Eclair** is a new project that came to replace old audio players to bring something new
to audio players, to show what can be done better and more beautifully.

## Why Eclair open-source?
**Eclair** is my hobby, as is programming in general.
I like to share the source code of the project and I don't see the point in closing the source code of a free
application.

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

# [License](LICENSE)
**Eclair** is licensed under the [MIT](https://choosealicense.com/licenses/mit). View the [LICENSE](LICENSE) file.