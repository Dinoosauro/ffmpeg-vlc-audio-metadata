# VLC and ffmpeg encoders

Here’s a table with a few audio encoders and extensions to use with ffmpeg-audio-metadata. 

<aside>
💡 Remember to pass both --codec and --extension as arguments when opening the application from the command line.

</aside>

| Audio format: | ffmpeg encoders: | VLC encoders: | Extension: |
| --- | --- | --- | --- |
| MP3 | libmp3lame | mp3 | mp3 |
| Advanced Audio Coding (the M4A/MP4 files) | - aac (more common)<br />- libfdk_aac (best audio quality, requires an ffmpeg compatible build) | mp4<br />Note: there might be some errors in the metadata passing. | m4a / mp4 |
| Opus (the best one) | libopus | opus | ogg |
| Vorbis | libvorbis | vorbis | ogg |
| Free Lossless Audio Codec | flac | flac | ogg / flac |
| Speex | libspeex (might require a specific ffmpeg build) | spx (even if it seems broken) | ogg |
| Wave |  | wav | wav |
| Windows Media Audio | wmav2<br />wmav1 | WMA2<br />WMA | wma |

Even if not all these combinations were tested, they should work. There are lots of more combinations for other, less common (I think), audio codecs. You can look them in the VLC or ffmpeg Wiki.