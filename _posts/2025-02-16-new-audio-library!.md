---
layout: post
title: New audio library!
permalink: /post/2025/02/16/new-audio-library
category: news
lang: en
---

Yes! I finally switched to another library! I am definitely happy about this. A lot has changed now (mostly on the technical side).

I decided to switch to the [BASS](https://un4seen.com) library. Yes, instead of OpenAL and many other audio libraries, I chose BASS, or more precisely [ManagedBass](https://github.com/ManagedBass/ManagedBass). I didn't think that transferring an audio player to another audio library would be easy and quite fast. I managed to do it in about 2 days. I was even almost able to implement the visualization of sound waves! More precisely, I was able to visualize them, but there are a couple of problems that need to be solved. I placed the visualization itself in "full screen" mode above the music icon. I placed it for testing, it is clear that in the release it cannot be left in the same position. And it needs to be put somewhere.

I have the following idea:<br>
If the round icon is enabled, then the visualization will be rounded and will be displayed according to the diameter of the icon. And if it is disabled, then the visualization will be blurred and will be displayed on the background.
Maybe something will change.

Also, if I manage to do this, then we will be able to see a miniature visualization of sound waves on top of the mini-player.