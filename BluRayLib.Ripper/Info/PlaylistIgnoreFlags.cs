namespace BluRayLib.Ripper.Info;

[Flags]
public enum PlaylistIgnoreFlags
{
    None = 0,
    TooShort = 1 << 0,
    TooLong = 1 << 1,
    Menu = 1 << 2,
    NoAudio = 1 << 3,
    NoSubtitle = 1 << 4,
    RepeatingClips = 1 << 5,
    Duplicate = 1 << 6,
}