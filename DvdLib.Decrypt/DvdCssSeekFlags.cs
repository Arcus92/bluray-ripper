namespace DvdLib.Decrypt;

[Flags]
public enum DvdCssSeekFlags : int
{
    None = 0,
    Mpeg = 1 << 0,
    Key = 1 << 1,
}