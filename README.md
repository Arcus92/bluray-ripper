# MediaRipper

This is a graphical interface to archive movies and TV shows from BluRays.

It uses the [MakeMKV](https://www.makemkv.com/) api to access the BluRay streams. However, this application allows you
to adjust the output format, codec, quality and input streams before exporting. This removes the need to export 

It also has a better detection for ignored tracks, like menu backgrounds or title cards.

This is a newer take on my [mkv-ripper](https://github.com/Arcus92/mkv-ripper) project.

## Work in progress
1
This is a very early proof-of-concept. Exporting and re-encoding is already working for `.mp4` and `.mkv`.

The UI is currently _non-functional_. 

### Planned features

The main goal is to speed up the manual task of title to episode or extra mapping.

- **Title preview** - Preview a track in an internal or external player _(for example VLC Player)_ direct from the disc 
without exporting.
- **Output manager** - Rename output files like episode titles, create sub-folders for extras, behind the scenes and more. Also rename subtitle tracks.
- **TheMovieDB integration** - Pulling episode titles and description by entering the season and episode number.
- **Subtitle detection** - Auto-detect close-capture, forced subtitles and commentary tracks by file size.
- **External subtitle** - Exporting graphical subtitle as separate `.sub` files.
- **Export disc mapping** - Storing the manual track mapping per disc. _Maybe we could create an online repository with pre-defined configurations._