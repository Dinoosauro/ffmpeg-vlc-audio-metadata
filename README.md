# ffmpeg-vlc-audio-metadata
Convert from an audio file to another using ffmpeg. Plus, parse all of the metadata from the input to the output.
## Configuration
[ffmpeg](https://ffmpeg.org/download.html) or [VLC](https://www.videolan.org/vlc/) is required. On Windows, download the .exe file and put it in the same folder of FfmpegAudioMetadata. On Linux/macOS, you can install ffmpeg also from a package manager. FfmpegAudioMetadata will ask you where you installed ffmpeg.
### Download
If you have .NET 6 Runtime installed, look [here](https://github.com/Dinoosauro/FfmpegAudioMetadata/releases/tag/release-dependent-1.1.0) for lightweight versions. If you don't have .NET 6 Runtime installed, or you are unsure, you can download a version that includes .NET [here](https://github.com/Dinoosauro/FfmpegAudioMetadata/releases/latest)

If you don't trust / don't want to use the pregenerated releases, that's fine. You can create your own build by using the .NET 6 SDK.
## Use
Open ffmpeg-audio-metadata and, when asked, write the file you want to convert. If you want to convert the entire folder, write `--everything *fileextension*`, for example, if you want to convert all the MP3 files, `--everything mp3`. FfmpegAudioMetadata will automtically convert the file(s).
## Arguments
- ` --codec`: specify the library used by ffmpeg to convert the file (example ` --codec libvorbis`). Default: libopus
- ` --bitrate`: specify the bitrate of a file (example ` --codec 192` for 192kbit/s). Default: the same of the input file. 
- ` --extension`: the extension the file will have (example ` .m4a` for MP4 Audio). Make sure to change it if you change the codec, to avoid putting certain codecs in incompatible containers. Default: ogg
- ` --vlc`: use VLC instead of ffmpeg for the audio conversion
- ` --customtimeout`: set a custom timeout if VLC conversion seems blocked.
- ` --codecguide`: show the guide for the audio codecs.
- ` --additional`: add additional values to ffmpeg
- ` --additionalcustom`: add additional vlues to ffmpeg, replacing also the default ones for OGG and AAC encoding.
- ` --printversion`: displays the version of FfmpegAudioMetadata
- ` --opensource`: shows the open source licenses of the libraries used
- ` --license`: shows FfmpegAudioMetadata's license
## Why this app
ffmpeg has some issues with MP3 (lyrics aren't converted correctly), MP4 (album art is dropped if not converted from MP3 or similar) and OGG (album art is always dropped). So, with this app, TagLib# is used to correctly copy all the metadata. This has been useful to me, and I think it can be useful also to others. 
