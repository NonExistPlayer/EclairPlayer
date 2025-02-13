---
layout: post
title: Migrating from LibVLCSharp?
permalink: /post/2025/02/13/migrating-from-libvlcsharp
category: post
lang: en
---
I've been thinking about using another audio library or even creating my own for a long time.

# What don't I like about LibVLCSharp?
It's all about visualizing sound waves in my player, or rather why there aren't any.
It's all because of the complexity and, perhaps, because the original LibVLC library is written in C/C++.
The thing is that to visualize sound waves, you obviously need audio data. LibVLCSharp provides slightly limited capabilities for obtaining audio data. After all, you can only intercept audio data, and when intercepted, LibVLC will not play music on its own. And implementing playback manually is difficult, you'll have to write a lot of platform-dependent code and it's easier to create your own audio library. There is an alternative option, these are audio filters. But to control them, you need to write code in C/C++, which, firstly, I don’t want to do, and secondly, I need to render this data through the Avalonia framework, and in turn, Avalonia only supports C#. Because of this, it turns out that it is necessary to implement the visualization of audio waves, which most audio players have, and given that my player is aimed at beauty, this **must** be implemented.
# So what should I do?
I decided to ask the **DeepSeek** AI which libraries can be chosen for this.
<details>

<summary>Prompt (russian)</summary>

*Привет! Я делаю свой аудио плеер на фреймворке Avalonia и ЯП C#.
В качестве аудио библиотеки я выбрал LibVLCSharp, но он мне не подходит из-за не возможночти реализовать визуализацию звуковых волн.
Подскажи мне несколько кросс-платформенных (Windows, Linux, Android) аудио библиотек, которые позволяют программировать на C# и предоставляют аудио волны для их визуализации в Avalonia.*

</details>

Here is her answer:

1. NAudio
2. CSCore
3. Bass
4. OpenTK (OpenAL)
5. SoundTouch
6. FFmpeg.AutoGen

<details>

<summary>Full answer (russian)</summary>

<hr style="border-top: 1.5px soild #bbb"><br>

Привет! Для создания аудиоплеера с визуализацией звуковых волн на C# и Avalonia, можно рассмотреть следующие кросс-платформенные библиотеки:

### 1. **NAudio**
   - **Описание**: NAudio — это популярная библиотека для работы со звуком на C#. Она поддерживает множество аудиоформатов и предоставляет доступ к аудиоданным, что позволяет реализовать визуализацию звуковых волн.
   - **Плюсы**:
     - Хорошая документация и активное сообщество.
     - Поддержка Windows, Linux (через .NET Core/.NET 5+) и Android (с некоторыми ограничениями).
     - Возможность работы с сырыми аудиоданными для визуализации.
   - **Минусы**:
     - На Android может потребоваться дополнительная настройка.
   - **Ссылка**: [NAudio GitHub](https://github.com/naudio/NAudio)

### 2. **CSCore**
   - **Описание**: CSCore — это библиотека для работы со звуком, которая предоставляет доступ к аудиоданным и поддерживает визуализацию. Она работает на Windows, Linux (через .NET Core) и Android.
   - **Плюсы**:
     - Поддержка множества аудиоформатов.
     - Возможность работы с аудиоданными для визуализации.
   - **Минусы**:
     - Меньше документации по сравнению с NAudio.
   - **Ссылка**: [CSCore GitHub](https://github.com/filoe/cscore)

### 3. **Bass**
   - **Описание**: Bass — это мощная библиотека для работы со звуком, которая поддерживает множество платформ, включая Windows, Linux и Android. Она предоставляет API для работы с аудиоданными, что позволяет реализовать визуализацию.
   - **Плюсы**:
     - Высокая производительность.
     - Поддержка множества форматов и эффектов.
   - **Минусы**:
     - Требуется покупка лицензии для коммерческого использования.
   - **Ссылка**: [Bass Audio Library](https://www.un4seen.com/)

### 4. **OpenTK (с использованием OpenAL)**
   - **Описание**: OpenTK — это библиотека для работы с графикой и звуком, которая использует OpenAL для аудио. Она поддерживает кросс-платформенную разработку и предоставляет доступ к аудиоданным.
   - **Плюсы**:
     - Полный контроль над аудиоданными.
     - Поддержка Windows, Linux и Android.
   - **Минусы**:
     - Более сложная в использовании по сравнению с другими библиотеками.
   - **Ссылка**: [OpenTK GitHub](https://github.com/opentk/opentk)

### 5. **SoundTouch**
   - **Описание**: SoundTouch — это библиотека для обработки звука, которая поддерживает изменение темпа и тональности. Она может быть использована для получения аудиоданных для визуализации.
   - **Плюсы**:
     - Поддержка кросс-платформенной разработки.
     - Возможность работы с аудиоданными.
   - **Минусы**:
     - Ограниченная функциональность для визуализации.
   - **Ссылка**: [SoundTouch GitHub](https://github.com/surge-synthesizer/soundtouch)

### 6. **FFmpeg.AutoGen**
   - **Описание**: FFmpeg.AutoGen — это обертка для FFmpeg, которая позволяет работать с аудио и видео данными. Она поддерживает множество форматов и предоставляет доступ к аудиоданным для визуализации.
   - **Плюсы**:
     - Мощная и гибкая библиотека.
     - Поддержка множества форматов.
   - **Минусы**:
     - Более сложная в использовании.
   - **Ссылка**: [FFmpeg.AutoGen GitHub](https://github.com/Ruslan-B/FFmpeg.AutoGen)

### Рекомендации:
- Если вам нужна простота и хорошая документация, начните с **NAudio**.
- Если требуется высокая производительность и поддержка множества форматов, рассмотрите **Bass**.
- Если вы готовы к более сложной настройке, но хотите полный контроль над аудиоданными, попробуйте **OpenTK** или **FFmpeg.AutoGen**.

Удачи с вашим аудиоплеером! Если будут вопросы, обращайтесь. 😊

<hr style="border-top: 1.5px soild #bbb">

</details>

We will immediately discard NAudio and CSCore because they are platform-dependent and officially support only Windows. According to the neural network, they can be cross-platform, but under certain conditions. It will be easier for me to use another library than to bother with NAudio and CSCore.<br>
We have the following libraries to choose from:

1. *Bass*
2. *OpenTK (OpenAL)*
3. SoundTouch
4. FFmpeg.AutoGen

I have already tried to work with those that I highlighted in italics, but there were problems. I don’t remember what problems OpenAL had, but I remember well what problems Bass had.
As far as I understand, Bass was not supported on Windows x86. Now I have Linux, maybe the problem will go away, but my player is cross-platform, and it may not work on Windows. In general, we will need to think about this.
We will also remove SoundTouch because, according to the neural network, there may be difficulties with visualization. FFmpeg.AutoGen can also be discarded because, again, according to the neural network, this audio library is difficult to use.<br>
So we are left with:

- Bass
- OpenAL

I will try both of these libraries starting with OpenAL. Then, after I finally decide on the library and remake the player to work with it, I will write a post (news) about it.